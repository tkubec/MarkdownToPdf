// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using MigraDoc.DocumentObjectModel;

namespace Orionsoft.MarkdownToPdfLib.Styling
{
    /// <summary>
    /// Part of <see cref="BorderStyle"/> defining border of one side (e.g. left, right, ...)
    /// </summary>
    public class SingleBorderStyle
    {
        public MigraDoc.DocumentObjectModel.BorderStyle? LineStyle { get; set; }
        public Dimension Width { get; set; }
        public Color Color { get; set; }

        internal SingleBorderStyle()
        {
            Width = new Dimension();
        }

        internal SingleBorderStyle MergeWith(SingleBorderStyle baseStyle)
        {
            var res = new SingleBorderStyle
            {
                Color = !Color.IsEmpty ? Color : baseStyle.Color,
                Width = !Width.IsEmpty ? Width : baseStyle.Width,
                LineStyle = LineStyle.HasValue ? LineStyle : baseStyle.LineStyle
            };
            return res;
        }

        internal void WriteTo(Border border, double fontSize, double containerWidth)
        {
            border.Color = Color.IsEmpty ? Colors.Black : Color;
            border.Width = Width.Eval(fontSize, containerWidth);
            border.Style = LineStyle ?? MigraDoc.DocumentObjectModel.BorderStyle.Single;
        }

        public bool HasValue()
        {
            if (Width.IsEmpty) return false;
            return true;
        }
    }
}