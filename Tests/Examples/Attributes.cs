using Orionsoft.MarkdownToPdfLib;
using Orionsoft.MarkdownToPdfLib.Styling;
using System;
using System.IO;

namespace Tests.Examples
{
    /// <summary>
    /// Demonstration of:
    /// </summary>

    public static class Attributes
    {
        public static void Run()
        {
            var markdown = File.ReadAllText("../../data/attributes.md");

            var pdf = new MarkdownToPdf();
            pdf.WarningIssued += (o, e) => { Console.WriteLine($"{e.Category}: {e.Message}"); };

            var style = pdf.StyleManager.AddStyle("myList", MarkdownStyleNames.UnorderedListItem);
            style.Bullet.Normal.Content = "*";
            pdf.StyleManager.ForElement(ElementType.UnorderedListItem).WithParent(ElementType.UnorderedList, "myList").Bind(style);
            pdf
             .Add(markdown)
             .Save("attributes.pdf");
        }
    }
}