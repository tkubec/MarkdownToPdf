// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig.Extensions.Tables;
using MigraDoc.DocumentObjectModel.Tables;
using Orionsoft.MarkdownToPdfLib.Styling;

namespace Orionsoft.MarkdownToPdfLib.Converters
{
    internal class TableCellConverter : ContainerBlockConverter, IStandaloneContainerConverter
    {
        public TableCell CurrentBlock { get; }

        public int Column { get; internal set; }

        public Cell OutputTableCell { get; private set; }

        internal TableCellConverter(TableCell node, ContainerBlockConverter parent)
            : base(node, parent)
        {
            ElementDescriptor = new SingleElementDescriptor { Attributes = Attributes, Type = ElementType.TableCell, Position = new ElementPosition(Block) };
            CurrentBlock = node;
        }

        protected override bool CreateOutput()
        {
            OutputTableCell = (Parent as TableRowConverter).OutputTableRow.Cells[Column];
            return true;
        }

        protected override void PrepareStyling()
        {
            base.PrepareStyling();
            EvaluatedStyle.Table.VerticalCellAlignment =
                EvaluatedStyle.Table.VerticalCellAlignment.HasValue ? EvaluatedStyle.Table.VerticalCellAlignment : Parent.EvaluatedStyle.Table.VerticalCellAlignment;

            if (!Parent.Parent.EvaluatedStyle.Table.GetColumnStyle(Column).Background.IsEmpty && EvaluatedStyle.Background.IsEmpty)
            {
                EvaluatedStyle.Background = Parent.Parent.EvaluatedStyle.Table.GetColumnStyle(Column).Background;
            }

            EvaluatedStyle.Font = Parent.Parent.EvaluatedStyle.Table.GetColumnStyle(Column).Font.MergeWith(EvaluatedStyle.Font);

            EvaluatedStyle.Border = EvaluatedStyle.Border.MergeWith(Parent.EvaluatedStyle.Border);

            // alignment setting from md markup:

            var horizontalAlignment = (Parent.Parent.Block as Markdig.Extensions.Tables.Table).ColumnDefinitions[Column].Alignment;
            if (horizontalAlignment.HasValue)
            {
                switch (horizontalAlignment)
                {
                    case TableColumnAlign.Left: EvaluatedStyle.Paragraph.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Left; break;
                    case TableColumnAlign.Right: EvaluatedStyle.Paragraph.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Right; break;
                    case TableColumnAlign.Center: EvaluatedStyle.Paragraph.Alignment = MigraDoc.DocumentObjectModel.ParagraphAlignment.Center; break;
                }
            }
        }

        protected override void ApplyStyling()
        {
            if (CurrentBlock.ColumnSpan > 1) OutputTableCell.MergeRight = CurrentBlock.ColumnSpan - 1;
            if (CurrentBlock.RowSpan > 1) OutputTableCell.MergeDown = CurrentBlock.RowSpan - 1;
            OutputTableCell.Borders = EvaluatedStyle.Border.MergeWithBorders(OutputTableCell.Borders, FontSize, Width);
            if (EvaluatedStyle.Table.VerticalCellAlignment.HasValue) OutputTableCell.VerticalAlignment = EvaluatedStyle.Table.VerticalCellAlignment.Value;
            OutputTableCell.Shading.Color = EvaluatedStyle.Background;
        }

        protected override void ConvertContent()
        {
            OutputContainer = new MigraDocBlockContainer(OutputTableCell, Owner);
            ConvertBlocks(CurrentBlock);
        }
    }
}