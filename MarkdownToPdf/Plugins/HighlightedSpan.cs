// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using System.Drawing;

namespace Orionsoft.MarkdownToPdfLib.Plugins
{
    /// <summary>
    /// Text span with formatting
    /// </summary>
    public class HighlightedSpan
    {
        public string Text { get; set; }
        public bool Bold { get; set; }
        public bool Italic { get; set; }
        public bool Underline { get; set; }
        public Color Color { get; set; }
    }
}