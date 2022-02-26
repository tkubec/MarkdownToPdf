using MigraDoc.DocumentObjectModel;
using Orionsoft.MarkdownToPdfLib;
using Orionsoft.MarkdownToPdfLib.Styling;
using System.IO;

namespace Tests.Examples
{
    /// <summary>
    /// Demonstration of:
    ///   - table formatting options
    ///   - column and row spans
    /// </summary>

    public static class Tables
    {
        public static void Run()
        {
            var markdown = File.ReadAllText("../../data/tables.md");

            var pdf = new MarkdownToPdf();

            // we define two columns in the default table, first with bold text
            var tableStyle = pdf.StyleManager.Styles[MarkdownStyleNames.Table];
            var col = tableStyle.Table.AddColumn();
            col.Width = "50%";
            col.Font.Bold = true;
            col = tableStyle.Table.AddColumn();
            col.Width = "50%";

            // zebra stripes table
            var zebraRowStyle = pdf.StyleManager.AddStyle("zebra", MarkdownStyleNames.TableRowOdd);
            zebraRowStyle.Background = Colors.AliceBlue;
            pdf.StyleManager.ForElement(ElementType.TableRowOdd).WithParent(ElementType.Table, "zebra").Bind(zebraRowStyle);

            // another table style, based on the defout one, changing first column background
            var firstColStyle = pdf.StyleManager.AddStyle("firstCol", MarkdownStyleNames.Table);
            var fcol = firstColStyle.Table.AddColumn();
            fcol.Background = Colors.Beige;
            pdf.StyleManager.ForElement(ElementType.Table, "firstCol").Bind(firstColStyle);

            pdf
             .Add(markdown)
             .Save("tables.pdf");
        }
    }
}