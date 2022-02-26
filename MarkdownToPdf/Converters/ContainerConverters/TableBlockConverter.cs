// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig.Extensions.Tables;
using Markdig.Syntax;
using MigraDoc.DocumentObjectModel;
using Orionsoft.MarkdownToPdfLib.Styling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orionsoft.MarkdownToPdfLib.Converters
{
    internal class TableBlockConverter : ContainerBlockConverter
    {
        private bool tableWidthSet;
        private List<Dimension> attributeColumnWidths;

        internal Table CurrentBlock { get; }
        public MigraDoc.DocumentObjectModel.Tables.Table OutputTable { get; private set; }

        internal TableBlockConverter(Table block, ContainerBlockConverter parent)
            : base(block, parent)
        {
            Attributes = new ElementAttributes(CustomGetTextBefore());
            ElementDescriptor = new SingleElementDescriptor { Attributes = Attributes, Type = ElementType.Table, Position = new ElementPosition(Block) };
            CurrentBlock = Block as Table;
        }

        protected override bool CreateOutput()
        {
            OutputTable = OutputContainer.AddTable();
            return true;
        }

        protected override void PrepareStyling()
        {
            base.PrepareStyling();
            PrepareVerticalInheritedMargins();

            if (!EvaluatedStyle.Table.Width.IsEmpty)
            {
                Width = EvaluatedStyle.Table.Width.Eval(FontSize, Parent.Width);
                tableWidthSet = true;
            }
            if (Attributes.ContainsKey("align"))
            {
                var align = Attributes["align"];

                switch (align)
                {
                    case "left": EvaluatedStyle.Table.HorizontalAlignment = MigraDoc.DocumentObjectModel.Tables.RowAlignment.Left; break;
                    case "right": EvaluatedStyle.Table.HorizontalAlignment = MigraDoc.DocumentObjectModel.Tables.RowAlignment.Right; break;
                    case "center": EvaluatedStyle.Table.HorizontalAlignment = MigraDoc.DocumentObjectModel.Tables.RowAlignment.Center; break;
                }
            }
            if (Attributes.ContainsKey("width"))
            {
                try
                {
                    var w = Dimension.Parse(Attributes.Attributes["width"]);
                    Width = w.Eval(FontSize, Parent.Width);
                    tableWidthSet = true;
                }
                catch (ArgumentException e)
                {
                    Owner.OnWarningIssued(this, "Table", "Invalid width " + e.Message + $", line { Block.Line}");
                }
            }

            PrepareColumnStyling();
        }

        private void PrepareColumnStyling()
        {
            if (Attributes.ContainsKey("columns"))
            {
                try
                {
                    var colDefs = Attributes.Attributes["columns"].Trim().Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var colWidths = new List<Dimension>();
                    foreach (var i in colDefs)
                    {
                        colWidths.Add(Dimension.Parse(i));
                    }
                    attributeColumnWidths = colWidths;
                }
                catch (ArgumentException e)
                {
                    Owner.OnWarningIssued(this, "Table", "Invalid column width " + e.Message + $", line {Block.Line}");
                }
            }
        }

        protected override void PrepareVerticalInheritedMargins()
        {
            base.PrepareVerticalInheritedMargins();

            EvaluatedStyle.Margin.Left += EvaluatedStyle.Padding.Left;
            EvaluatedStyle.Margin.Right += EvaluatedStyle.Padding.Right;
            EvaluatedStyle.Padding.Left = 0;
            EvaluatedStyle.Padding.Right = 0;

            EvaluatedStyle.Margin.Left = EvaluatedStyle.Margin.Left.IsEmpty ? 0 : EvaluatedStyle.Margin.Left;
            EvaluatedStyle.Margin.Right = EvaluatedStyle.Margin.Left.IsEmpty ? 0 : EvaluatedStyle.Margin.Right;
        }

        protected override void AdjustInheritedHorizontalMargins()
        {
            topMarginsPending = ApplyHorizontalInheritedMargin(BoxSide.Top);
            bottomMarginsPending = ApplyHorizontalInheritedMargin(BoxSide.Bottom);
        }

        protected override void ApplyStyling()
        {
            OutputTable.Rows.Alignment = EvaluatedStyle.Table.HorizontalAlignment ?? OutputTable.Rows.Alignment;
            OutputTable.Rows.LeftIndent = EvaluatedStyle.Margin.Left.Eval(FontSize, Width);
            OutputTable.LeftPadding = EvaluatedStyle.Table.CellSpacing.Left.Eval(FontSize, Width);
            OutputTable.RightPadding = EvaluatedStyle.Table.CellSpacing.Right.Eval(FontSize, Width);
            OutputTable.TopPadding = EvaluatedStyle.Table.CellSpacing.Top.Eval(FontSize, Width);
            OutputTable.BottomPadding = EvaluatedStyle.Table.CellSpacing.Bottom.Eval(FontSize, Width);

            if (!EvaluatedStyle.Background.IsEmpty) OutputTable.Shading.Color = EvaluatedStyle.Background;
        }

        protected override void ConvertContent()
        {
            PrepareColumns();
            var topMargin = EvaluatedStyle.Margin.Top.Eval(FontSize, Width);
            var bottomMargin = EvaluatedStyle.Margin.Top.Eval(FontSize, Width);
            AddSimulatedMargin(topMargin, top: true);

            foreach (var c in CurrentBlock)
            {
                ConvertBlock(c);
            }

            AddSimulatedMargin(bottomMargin);

            // main border
            if (!EvaluatedStyle.Border.Width.IsEmptyOrZero(FontSize, Width))
            {
                // hide simulated margins:
                var first = topMargin.IsEmpty ? 0 : 1;
                var count = OutputTable.Rows.Count - (topMargin.IsEmpty ? 0 : 1) - (bottomMargin.IsEmpty ? 0 : 1);

                OutputTable.SetEdge(0, first, OutputTable.Columns.Count, count, MigraDoc.DocumentObjectModel.Tables.Edge.Box,
                    EvaluatedStyle.Border.LineStyle ?? MigraDoc.DocumentObjectModel.BorderStyle.Single, EvaluatedStyle.Border.Width.Eval(FontSize, Width), EvaluatedStyle.Border.Color);
            }
        }

        private void AddSimulatedMargin(Unit margin, bool top = false)
        {
            if (!margin.IsEmpty)
            {
                var fakeRow = OutputTable.AddRow();
                if (top) fakeRow.HeadingFormat = true;
                fakeRow.Height = margin - EvaluatedStyle.Table.CellSpacing.Top.Eval(FontSize, Width) - -EvaluatedStyle.Table.CellSpacing.Bottom.Eval(FontSize, Width);
                fakeRow.Shading.Color = EvaluatedStyle.Background;
            }
        }

        private void PrepareColumns()
        {
            var colWidths = GetColumnWidths();
            var totalColumnsWidth = colWidths.Sum(x => x.Eval(FontSize, Width));
            var scale = totalColumnsWidth > Width || tableWidthSet ? Width / totalColumnsWidth : 1.0;

            for (var i = 0; i < colWidths.Count; i++)
            {
                var cl = OutputTable.Columns.AddColumn();
                cl.Width = colWidths[i].Eval(FontSize, Width) * scale;
            }
        }

        private List<Dimension> GetColumnWidths()
        {
            var colCount = CurrentBlock.ColumnDefinitions.Count;

            // Ignore empty trailing column in  pipe tables
            var isPipe = ((CurrentBlock.First() as TableRow).First() as TableCell).ColumnIndex == -1;
            colCount = isPipe ? (CurrentBlock.First() as TableRow).Count : colCount;
            var res = new List<Dimension>();

            for (var i = 0; i < colCount; i++)
            {
                if (attributeColumnWidths != null)
                {
                    var d = attributeColumnWidths.Any() ? attributeColumnWidths[Math.Min(attributeColumnWidths.Count - 1, i)] : Dimension.FromContainerWidth(100);
                    res.Add(d);
                }
                else
                {
                    var colStyle = EvaluatedStyle.Table.Columns.Any() ? EvaluatedStyle.Table.Columns[Math.Min(EvaluatedStyle.Table.Columns.Count - 1, i)] : new TableColumnStyle { Width = "100%" };
                    res.Add(colStyle.Width);
                }
            }
            return res;
        }

        protected override bool ConvertBlock(Block block)
        {
            var type = block.GetType();

            if (type == typeof(TableRow))
            {
                var conv = new TableRowConverter(block as TableRow, this)
                {
                };

                conv.Convert();
                return true;
            }

            return false;
        }

        protected string CustomGetTextBefore()
        {
            if (Block.Parser is GridTableParser)
            {
                return Block.Line > 0 ? Lines[Block.Line - 1] : "";
            }
            else
            {
                return GetTextBefore();
            }
        }
    }
}