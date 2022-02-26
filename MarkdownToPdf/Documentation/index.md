# MarkdownToPdf Library

This C# library provides an easy way how to generate PDF files from markdown documents.

In the simplest case, you just need this code:

```csharp
var pdf = new MarkdownToPdf();

pdf
    .Add("## Hello World\r\n\r\nHello!")
    .Save("hello.pdf");
```

To learn the basics, please see the **examples** and read the [articles](articles/intro.md). For more details, see  the library [API documentation](api/index.md).


The recommended reading order of examples:

 - HelloWorld
 - BasicStyling
 - CustomStyles
 - AdvancedStyling
 - Attributes
 - Tables
 - Sections
 - Toc
 - Events
 - Features
 
