# Usage

The basic usage is pretty straightforward:

- create an instance of `MarkdownToPdf`
- optionally set-up the [page layout](pagesetup.md)
- optionally modify or add [styles](styles.md)
- add the markdown text
- save

E.g.:

```csharp

var pdf = new MarkdownToPdf();

pdf
    .PaperSize(PaperSize.B5)
    .DefaultFont("Garamond", 12)
    .Add(markdown)
    .Save("alice.pdf");

```
Headers and footers can be added by `AddHeader()` and `AdFooter()` and their variants - see the [MarkDownToPdfClass](xref:Orionsoft.MarkdownToPdfLib.MarkdownToPdf) documentation. Like in this example:

```csharp
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

```