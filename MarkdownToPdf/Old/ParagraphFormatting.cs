using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownToPdf
{
    public struct ParagraphFormatting
    {
        public bool BulletList { get; set; }
        public Unit Indent { get; set; }
    }
}
