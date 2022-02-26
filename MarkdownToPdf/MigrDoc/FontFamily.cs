// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

namespace Orionsoft.MarkdownToPdfLib
{
    internal class FontFamily
    {
        public string Name { get; set; }
        public string Normal { get; set; }
        public string Bold { get; set; }
        public string Italic { get; set; }
        public string BoldItalic { get; set; }

        public FontFamily(string name, string regular, string bold = "", string italic = "", string boldItalic = "")
        {
            Name = name;
            Normal = regular;
            Bold = bold.HasValue() ? bold : Normal;
            Italic = italic.HasValue() ? italic : Normal;
            BoldItalic = boldItalic.HasValue() ? boldItalic : italic.HasValue() ? italic : regular;
        }
    }
}