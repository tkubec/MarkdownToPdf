using Orionsoft.MarkdownToPdfLib;
using System;
using System.IO;

namespace Tests.Examples
{
    /// <summary>
    /// Playground
    /// </summary>

    public static class Highlighting
    {
        public static void Run()
        {
            var markdown = File.ReadAllText("../../data/highlighting.md");
            var pdf = new MarkdownToPdf();
            pdf.PluginManager.Add(new DemoHighlighter.Highlighter());
            pdf.WarningIssued += (o, e) => { Console.WriteLine($"{e.Category}: {e.Message}"); };

            pdf.Add(markdown)

            .Save("highlighting.pdf");
        }
    }
}