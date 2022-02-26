using MigraDoc.DocumentObjectModel;
using Orionsoft.MarkdownToPdfLib;
using Orionsoft.MarkdownToPdfLib.Styling;
using System.IO;

namespace Tests.Examples
{
    /// <summary>
    /// Demonstration of:
    ///   - different styling based on different markdown markup - asterism style horizontal break
    ///   - style modification based on binding condition - last item of list with red text
    ///   - conditional formatting
    /// </summary>

    public static class AdvancedStyling
    {
        public static void Run()
        {
            var markdown = File.ReadAllText("../../data/advanced.md");

            var pdf = new MarkdownToPdf();

            // definition of asterism ruler - centered decorative symbol
            var asterismStyle = pdf.StyleManager.AddStyle("asterismRuler", MarkdownStyleNames.Break);
            asterismStyle.Paragraph.Alignment = ParagraphAlignment.Center;
            asterismStyle.Bullet.Normal.Content = "\xF9";
            asterismStyle.Bullet.Normal.Font.Name = "Wingdings 2";
            asterismStyle.Bullet.Normal.Font.Size = "2em";
            asterismStyle.Margin.Top = 12;
            asterismStyle.Margin.Bottom = 12;
            asterismStyle.Border.Bottom.Width = 0;

            // asterism style is bound accorrding to markup sytnax variant of horizontal rule  "***"
            pdf.StyleManager
                .ForElement(ElementType.Break)
                .Where(x => x.CurrentElement.Attributes.Markup == "*")
                .Bind(asterismStyle);

            // normal UnorderedListItem style is bound to unordered list item, but for last items it is modified to use red color
            pdf.StyleManager
                .ForElement(ElementType.UnorderedListItem)
                .Where(x => x.CurrentElement.Position.IsLast)
                .BindAndModify(MarkdownStyleNames.UnorderedListItem, (s, d) => s.Font.Color = Colors.Red);

            // normal Paragraph style is bound to paragraph within table, but in case the content is a negative number it is modified to use red color
            pdf.StyleManager
                .ForElement(ElementType.Paragraph)
                .WithParent(ElementType.TableCell)
                .Where(x => double.TryParse(x.CurrentElement.PlainText, out double result) && result < 0)
                .BindAndModify(MarkdownStyleNames.Paragraph, (s, d) => s.Font.Color = Colors.Red);

            pdf
             .Add(markdown)
             .Save("advanced.pdf");
        }
    }
}