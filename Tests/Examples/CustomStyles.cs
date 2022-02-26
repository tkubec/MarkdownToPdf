using MigraDoc.DocumentObjectModel;
using Orionsoft.MarkdownToPdfLib;
using Orionsoft.MarkdownToPdfLib.Styling;
using System.IO;

namespace Tests.Examples
{
    /// <summary>
    /// Demonstration of:
    ///   - basic page setup
    ///   - default font
    ///   - footer with page numbering
    /// </summary>

    public static class CustomStyles
    {
        public static void Run()
        {
            var markdown = File.ReadAllText("../../data/customStyles.md");
            var pdf = new MarkdownToPdf();

            // style with modifyied bullet is bound to nested list item (list item with an ancestor - other list litem)
            var style = pdf.StyleManager.AddStyle("NestedListItem", MarkdownStyleNames.UnorderedListItem);
            style.Bullet.Normal.Content = "*";
            pdf.StyleManager.ForElement(ElementType.UnorderedListItem).WithAncestor(ElementType.UnorderedListItem).Bind(style);

            // custom paragraph style "Blue" is created and bound to paragraphs with syle name "blue"
            style = pdf.StyleManager.AddStyle("Blue", MarkdownStyleNames.Paragraph);
            style.Font.Color = Colors.Blue;
            style.Paragraph.Alignment = ParagraphAlignment.Justify;
            pdf.StyleManager.ForElement(ElementType.Paragraph, "blue").Bind(style);

            pdf
             .DefaultFont("Calibri", 11)
             .Add(markdown)
             .Save("customStyles.pdf");
        }
    }
}