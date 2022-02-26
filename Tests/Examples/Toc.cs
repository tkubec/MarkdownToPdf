using Orionsoft.MarkdownToPdfLib;
using System.IO;

namespace Tests.Examples
{
    /// <summary>
    /// Demonstration of:
    /// use of .Toc styles to render TOC lines with page number
    /// </summary>

    public static class Toc
    {
        public static void Run()
        {
            var markdown = File.ReadAllText("../../data/toc.md");

            var pdf = new MarkdownToPdf();

            pdf
             .PaperSize(PaperSize.A5)
             .Add(markdown)
             .Save("toc.pdf");
        }
    }
}