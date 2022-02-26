// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using MigraDoc.DocumentObjectModel;

namespace Orionsoft.MarkdownToPdfLib.Styling
{
    public class BordersStyle
    {
        public BorderStyle? Style { get; set; }
        public Unit Width { get; set; }
        public Color Color { get; set; }

        public SingleBorderStyle Top { get; set; }
        public SingleBorderStyle Bottom { get; set; }
        public SingleBorderStyle Left { get; set; }
        public SingleBorderStyle Right { get; set; }

        internal BordersStyle()
        {
            Top = new SingleBorderStyle();
            Bottom = new SingleBorderStyle();
            Left = new SingleBorderStyle();
            Right = new SingleBorderStyle();
        }

        public bool HasValue()
        {
            if (!Color.IsEmpty) return true;
            if (!Width.IsEmpty) return true;
            if (Style.HasValue) return true;
            return false;
        }
        internal BordersStyle ApplyTo(BordersStyle style, bool fillUndefinedSides = false)
        {
            var res = new BordersStyle
            {
                Color = !Color.IsEmpty ? Color : style.Color,
                Width = !Width.IsEmpty ? Width : style.Width,
                Style = Style.HasValue ? Style : style.Style,

                Top = Top.ApplyTo(style.Top),
                Bottom = Bottom.ApplyTo(style.Bottom),
                Left = Left.ApplyTo(style.Left),
                Right = Right.ApplyTo(style.Right)
            };

            if (fillUndefinedSides)
            {
                if (!Top.HasValue()) ApplyTo(res.Top);

                if (!Bottom.HasValue()) ApplyTo(res.Bottom);

                if (!Left.HasValue()) ApplyTo(res.Left);

                if (!Right.HasValue()) ApplyTo(res.Right);
            }
            return res;
        }

        internal Borders ApplyTo(Borders borders)
        {
            var res = borders.Clone();

            res.Color = !Color.IsEmpty ? Color : res.Color;
            res.Width = !Width.IsEmpty ? Width : res.Width;
            if (Style.HasValue) res.Style = Style.Value; // bug in Migradoc

            // Migradoc bug - if border is just read, it is set as existing and overrides the main border
            if (Bottom.HasValue()) Bottom.ApplyTo(res.Bottom);
            else if (HasValue()) ApplyTo(res.Bottom);

            if (Top.HasValue()) Top.ApplyTo(res.Top);
            else if (HasValue()) ApplyTo(res.Top);

            if (Left.HasValue()) Left.ApplyTo(res.Left);
            else if (HasValue()) ApplyTo(res.Left);

            if (Right.HasValue()) Right.ApplyTo(res.Right);
            else if (HasValue()) ApplyTo(res.Right);

            return res;
        }
        internal void ApplyTo(SingleBorderStyle border)
        {
            border.Color = Color;
            border.Width = Width;
            border.Style = Style;
        }

        internal void ApplyTo(Border border)
        {
            if (!Color.IsEmpty) border.Color = Color;
            if (!Width.IsEmpty) border.Width = Width;
            if (Style.HasValue) border.Style = Style.Value; // bug in Migradoc
        }
    }
}