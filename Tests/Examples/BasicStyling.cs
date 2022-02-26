using MigraDoc.DocumentObjectModel;
using Orionsoft.MarkdownToPdfLib;
using Orionsoft.MarkdownToPdfLib.Styling;
using System.IO;

namespace Tests.Examples
{
    /// <summary>
    /// Demonstration of:
    ///   - basic page setup
    ///   - changing default font
    ///   - changing built-instyle
    ///   - footer with page numbering
    /// </summary>

    public static class BasicStyling
    {
        public static void Run()
        {
            var markdown = File.ReadAllText("../../data/alice1.md");
            var footer = "{align=center}\r\n\\- [](md:page) - ";

            var pdf = new MarkdownToPdf();
            var paragraphStyle = pdf.StyleManager.Styles[MarkdownStyleNames.Paragraph];
            paragraphStyle.Paragraph.Alignment = ParagraphAlignment.Justify;
            paragraphStyle.Paragraph.FirstLineIndent = "1cm";

            pdf
             .PaperSize(PaperSize.B5)
             .Title("Alice's Adventures in Wonderland, Chapter I")
             .Author("Lewis Carroll")
             .DefaultFont("Garamond", 12)
             .Add(markdown)
             .AddFooter(footer)
             .Save("alice.pdf");
        }
    }
}