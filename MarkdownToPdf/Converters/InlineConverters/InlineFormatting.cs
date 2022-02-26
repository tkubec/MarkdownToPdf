using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownToPdf
{
    public struct InlineFormatting
    {
        public bool Bold { get; set; }
        public bool Italic { get; set; }

        public bool Superscript { get; set; }

        public bool Subscript { get; set; }
        public Font Font { get; internal set; }
    }
}
