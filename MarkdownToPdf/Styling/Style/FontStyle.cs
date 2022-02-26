// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using MigraDoc.DocumentObjectModel;

namespace Orionsoft.MarkdownToPdfLib.Styling
{
    /// <summary>
    /// Part of <see cref="CascadingStyle"/> defining font face, size, color and typeface style
    /// </summary>

    public class FontStyle
    {
        public string Name { get; set; }
        public Dimension Size { get; set; }
        public bool? Bold { get; set; }
        public bool? Italic { get; set; }
        public Underline? Underline { get; set; }
        public Color Color { get; set; }
        public bool? Superscript { get; set; }
        public bool? Subscript { get; set; }

        internal FontStyle()
        {
            Size = new Dimension();
        }

        internal FontStyle MergeWith(FontStyle baseStyle)
        {
            var res = new FontStyle
            {
                // if there is no font size defined, it is set to 1em, to prevent unwanted repeated scaling
                Name = Name.HasValue() ? Name : baseStyle.Name,
                Size = Size.IsEmpty ? Dimension.FromFontSize(1) : Size,
                Bold = Bold.HasValue ? Bold : baseStyle.Bold,
                Italic = Italic.HasValue ? Italic : baseStyle.Italic,
                Underline = Underline.HasValue ? Underline : baseStyle.Underline,
                Superscript = Superscript.HasValue ? Superscript : baseStyle.Superscript,
                Subscript = Subscript.HasValue ? Subscript : baseStyle.Subscript,
                Color = Color.IsEmpty ? baseStyle.Color : Color
            };

            return res;
        }

        internal Font MergeWithFont(Font font, double fontSize, double containerWidth, bool alreadyScaled)
        {
            var res = font.Clone();
            res.Name = Name.HasValue() ? Name : res.Name;
            res.Size = Size.IsEmpty ? res.Size : (alreadyScaled ? Unit.FromPoint(fontSize) : Size.Eval(fontSize, containerWidth));
            res.Color = Color.IsEmpty ? res.Color : Color;
            res.Bold = Bold ?? res.Bold;
            res.Italic = Italic ?? res.Italic;
            res.Underline = Underline ?? res.Underline;

            if (Superscript.HasValue || Subscript.HasValue)
            {
                res.Superscript = false;
                res.Subscript = false;
                if (Subscript.HasValue && Subscript.Value) res.Subscript = true;
                if (Superscript.HasValue && Superscript.Value) res.Superscript = true;
            }

            return res;
        }

        internal FontStyle Clone()
        {
            return new FontStyle
            {
                Name = Name,
                Size = Size,
                Bold = Bold,
                Italic = Italic,
                Underline = Underline,
                Superscript = Superscript,
                Subscript = Subscript,
                Color = Color
            };
        }
    }
}