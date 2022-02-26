using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownToPdf
{
    public class MarkdownStyle
    {
        public string Name { get; private set; }

        public ParagraphFormat ParagraphFormat { get; set; }

        public Font Font { get => ParagraphFormat.Font; set => ParagraphFormat.Font = value; }

        public MarkdownBulletStyle BulletStyle{ get; private set; }

        public MarkdownStyle(Document document, string name)
        {
            BulletStyle = new MarkdownBulletStyle();
        }
        public MarkdownStyle(Document document, string name, MarkdownStyle baseStyle = null) 
        {
            Name = name;
            if (baseStyle == null)
            {
                ParagraphFormat = document.Styles[StyleNames.InvalidStyleName].ParagraphFormat.Clone();
                ParagraphFormat.Font = new Font();
                BulletStyle = new MarkdownBulletStyle();
            }
            else
            {
                ParagraphFormat = baseStyle.ParagraphFormat.Clone();
                BulletStyle = new MarkdownBulletStyle(baseStyle.BulletStyle);
            }
        }
    }
}
