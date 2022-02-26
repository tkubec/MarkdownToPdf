// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using MigraDoc.DocumentObjectModel.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orionsoft.MarkdownToPdfLib.Styling
{
    /// <summary>
    /// Part of <see cref="CascadingStyle"/> defining table style
    /// </summary>

    public class TableStyle
    {
        private List<TableColumnStyle> columns;

        /// <summary>
        /// Alignnment of the entire table
        /// </summary>
        public RowAlignment? HorizontalAlignment { get; set; }

        /// <summary>
        /// Horizontal alignment of cell content. Can be directtly applied to a single cell as well
        /// </summary>

        public VerticalAlignment? VerticalCellAlignment { get; set; }

        public CellSpacingStyle CellSpacing { get; set; }

        /// <summary>
        /// Table width. If not defined, it is calucated from column widths
        /// </summary>
        public Dimension Width { get; set; }

        /// <summary>
        /// Collection of column definitions
        /// </summary>
        public IReadOnlyList<TableColumnStyle> Columns { get => columns.AsReadOnly(); }

        public TableStyle()
        {
            CellSpacing = new CellSpacingStyle();
            Width = new Dimension();
            columns = new List<TableColumnStyle>();
        }

        /// <summary>
        /// Adds column definition
        /// </summary>
        /// <returns></returns>
        public TableColumnStyle AddColumn()
        {
            var c = new TableColumnStyle();
            columns.Add(c);
            return c;
        }

        internal TableStyle MergeWith(TableStyle baseStyle)
        {
            var res = new TableStyle
            {
                HorizontalAlignment = HorizontalAlignment.HasValue ? HorizontalAlignment : baseStyle.HorizontalAlignment,
                VerticalCellAlignment = VerticalCellAlignment.HasValue ? VerticalCellAlignment : baseStyle.VerticalCellAlignment,
                Width = !Width.IsEmpty ? Width : baseStyle.Width,
            };

            res.CellSpacing = CellSpacing.MergeWith(baseStyle.CellSpacing);

            var colCount = Math.Max(baseStyle.columns.Count, columns.Count);
            res.columns = new List<TableColumnStyle>();
            for (var i = 0; i < colCount; i++)
            {
                if (i < baseStyle.Columns.Count && i < columns.Count)
                {
                    var t = Columns[i].MergeWith(baseStyle.Columns[i]);
                    res.columns.Add(t);
                }
                else if (i < Columns.Count)
                {
                    var t = Columns[i].Clone();
                    res.columns.Add(t);
                }
                else if (i < baseStyle.Columns.Count)
                {
                    var t = baseStyle.Columns[i].Clone();
                    res.columns.Add(t);
                }
            }

            return res;
        }

        internal TableColumnStyle GetColumnStyle(int index)
        {
            if (!columns.Any()) return new TableColumnStyle();

            return index >= columns.Count ? columns.Last() : columns[index];
        }
    }
}