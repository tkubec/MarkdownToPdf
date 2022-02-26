// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using MigraDoc.DocumentObjectModel;

namespace Orionsoft.MarkdownToPdfLib.Styling
{
    /// <summary>
    /// <b>Style that can be bound to markdown elements</b>
    /// </summary>
    /// <seealso cref="StyleManager"/>
    /// /// <seealso cref="SelectorBuilder.Bind(CascadingStyle)"/>
    public class CascadingStyle
    {
        public string Name { get; private set; }
        public CascadingStyle Parent { get; private set; }
        public FontStyle Font { get; set; }
        public Color Background { get; set; }
        public MarginStyle Margin { get; set; }
        public PaddingStyle Padding { get; set; }
        public BorderStyle Border { get; set; }
        public ParagraphStyle Paragraph { get; set; }
        public BulletStyle Bullet { get; set; }
        public TableStyle Table { get; set; }

        internal CascadingStyle(string name, CascadingStyle baseStyle = null)
        {
            Name = name;
            Parent = baseStyle;
            Font = new FontStyle();
            Margin = new MarginStyle();
            Padding = new PaddingStyle();
            Border = new BorderStyle();
            Paragraph = new ParagraphStyle();
            Bullet = new BulletStyle();
            Table = new TableStyle();
        }

        internal CascadingStyle Eval()
        {
            CascadingStyle res;

            if (Parent != null)
            {
                res = Parent.Eval();
            }
            else
            {
                res = new CascadingStyle("");
            }

            ApplyTo(res);
            res.Name = "Eval: " + Name;

            return res;
        }

        private void ApplyTo(CascadingStyle res)
        {
            res.Background = Background.IsEmpty ? res.Background : Background;
            res.Font = Font.MergeWith(res.Font);
            res.Paragraph = Paragraph.MergeWith(res.Paragraph);
            res.Margin = Margin.MergeWith(res.Margin);
            res.Padding = Padding.MergeWith(res.Padding);
            res.Border = Border.MergeWith(res.Border);
            res.Bullet = Bullet.MergeWith(res.Bullet);
            res.Table = Table.MergeWith(res.Table);
        }

        internal CascadingStyle Clone()
        {
            var res = new CascadingStyle(Name, Parent);
            ApplyTo(res);

            return res;
        }

        internal ParagraphFormat Merge(ParagraphFormat p, double fontSize, double containerWidth)
        {
            var res = p.Clone();
            res = Paragraph.Merge(res, fontSize, containerWidth);
            res.Font = Font.MergeWithFont(res.Font, fontSize, containerWidth, alreadyScaled: true);
            res.Shading.Color = res.Shading.Color.IsEmpty ? Background : res.Shading.Color;
            res.Borders = Border.MergeWithBorders(res.Borders, fontSize, containerWidth);
            return res;
        }
    }
}