// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using MigraDoc.DocumentObjectModel;

namespace Orionsoft.MarkdownToPdfLib.Styling
{
    /// <summary>
    /// Part of <see cref="TableStyle"/> defining columns and their styles
    /// </summary>
    public class TableColumnStyle
    {
        public ParagraphAlignment? HorizontalAlignment { get; set; }

        public Dimension Width { get; set; }

        public FontStyle Font { get; set; }

        public Color Background { get; set; }

        public TableColumnStyle()
        {
            Width = new Dimension();
            Font = new FontStyle();
        }

        internal TableColumnStyle MergeWith(TableColumnStyle baseStyle)
        {
            return new TableColumnStyle
            {
                Background = Background.IsEmpty ? baseStyle.Background : Background,
                HorizontalAlignment = HorizontalAlignment ?? baseStyle.HorizontalAlignment,
                Width = Width.IsEmpty ? baseStyle.Width : Width,
                Font = Font.MergeWith(baseStyle.Font)
            };
        }

        internal TableColumnStyle Clone()
        {
            return new TableColumnStyle { Background = Background, HorizontalAlignment = HorizontalAlignment, Width = Width, Font = Font.Clone() };
        }
    }
}