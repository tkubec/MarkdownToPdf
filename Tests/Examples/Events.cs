using MigraDoc.DocumentObjectModel;
using Orionsoft.MarkdownToPdfLib;
using Orionsoft.MarkdownToPdfLib.Converters;
using Orionsoft.MarkdownToPdfLib.Styling;
using System.IO;
using System.Text.RegularExpressions;

namespace Tests.Examples
{
    /// <summary>
    /// Demonstration of:
    /// - event ensuring that a line cannot break after a single letter word by adding NBSP to the output in front of it
    /// - custom formatting via an event (can be also easily done by a style, just an example)
    /// - implementation of a custom attribute "keep-with-previous" via en event modifying the migradoc output document

    /// </summary>

    public static class Events
    {
        public static void Run()
        {
            var markdown = File.ReadAllText("../../data/events.md");

            var pdf = new MarkdownToPdf();

            // in a text span containing a single letter word followed by another world, the space is replaced by a non-breaking space
            pdf.ConvertingLiteral += (o, e) => e.Text = Regex.Replace(e.Text, @"(?<=\b\w\b)\s(?=\w)", "\x00A0");

            // Custom formatting event
            pdf.StylingPrepared += CustomFormatter;

            // Event modifying the output migradoc document to keep a paragraph with previous one
            pdf.StylingApplied += KeepWithPrevious;

            pdf
             .Add(markdown)
             .Save("events.pdf");
        }

        private static void CustomFormatter(object sender, System.EventArgs e)
        {
            if (!(sender is IBlockConverter caller)) return;

            // if the element is paragraph within a table cell and it's text content is a negative number, we modify the style

            if (caller.Descriptor.CurrentElement.Type == ElementType.Paragraph && caller.Descriptor.HasParent(ElementType.TableCell)
                && double.TryParse(caller.Descriptor.CurrentElement.PlainText, out double res) && res < 0)
            {
                caller.EvaluatedStyle.Font.Color = Colors.Red;
            }
        }

        private static void KeepWithPrevious(object sender, System.EventArgs e)
        {
            if (!(sender is IBlockConverter caller && caller.Attributes.ContainsKey("keep-with-previous"))) return;

            var migraElements = caller.OutputContainer.Section?.Elements;

            // The current paragraph already exists, we need to modify the previous one:
            if (migraElements != null && migraElements.Count >= 2 && migraElements[migraElements.Count - 2] is Paragraph)
            {
                (migraElements[migraElements.Count - 2] as Paragraph).Format.KeepWithNext = true;
            }
        }
    }
}