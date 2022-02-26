# Page Setup

Page setup, such as setup of margins, page size, can be achieved via fluent styling methods of `MarkdownToPdf` class. E.g.

```csharp
pdf
    .PaperSize(PaperSize.B5)
    .Title("Alice's Adventures in Wonderland, Chapter I")
    .Author("Lewis Carroll")
    .DefaultFont("Garamond", 12)

```

If there is no document section yet, the settings are applied to DefaultSetup and can be reused  in sections created afterwards. If there is a section, it is applied to the current section.
