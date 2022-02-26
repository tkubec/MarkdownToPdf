using MigraDoc.DocumentObjectModel;
using Orionsoft.MarkdownToPdfLib;
using Orionsoft.MarkdownToPdfLib.Styling;
using System.IO;

namespace Tests.Examples
{
    /// <summary>
    /// Demonstration of:
    ///   - most markup features (without attributes)
    /// </summary>

    public static class Features
    {
        public static void Run()
        {
            var markdown = File.ReadAllText("../../data/features.md");
            var footer = "{align=center}\r\nPage [](md:page)";
            var pdf = new MarkdownToPdf();
            pdf.PluginManager.Add(new DemoHighlighter.Highlighter());

            // definition of custom styles used in the document
            var style = pdf.StyleManager.AddStyle("CustomListItem", MarkdownStyleNames.UnorderedListItem);
            style.Bullet.Normal.Content = "\x7B";
            style.Padding.Bottom = "12";
            style.Bullet.Normal.Font.Name = "Wingdings";

            style = pdf.StyleManager.AddStyle("NestedCustomContainer", MarkdownStyleNames.CustomContainer);
            style.Background = Colors.LightSalmon;
            pdf.StyleManager.ForElement(ElementType.CustomContainer).WithParent(ElementType.CustomContainer).Bind(style);

            pdf
             .PaperSize(PaperSize.A4)
             .FontDir("../../data/fonts")
             .RegisterLocalFont("Roboto", regular: "Roboto-Light.ttf", bold: "Roboto-Bold.ttf", italic: "Roboto-Italic.ttf")
             .DefaultFont("Roboto", 11)
             .Add(markdown)
             .AddFooter(footer)
             .Save("features.pdf");
        }
    }
}