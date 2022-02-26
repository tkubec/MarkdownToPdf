using DemoHighlighter;
using Orionsoft.MarkdownToPdfLib;
using System;
using System.IO;

namespace Tests.Examples
{
    /// <summary>
    /// Playground
    /// </summary>

    public static class SyntaxHighlighting
    {
        public static void Run()
        {
            var markdown = File.ReadAllText("../../data/test.md");
            var pdf = new MarkdownToPdf();
            var hl = new Highlighter();
            pdf.Plugins.Add(hl);

            pdf.WarningIssued += (o, e) => { Console.WriteLine($"{e.Category}: {e.Message}"); };
            
            pdf.Add(markdown)
            .Save("highlighting.pdf");
        }
    }
}