// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using MigraDoc.DocumentObjectModel;
using Orionsoft.MarkdownToPdfLib.Converters;
using Orionsoft.MarkdownToPdfLib.Styling;
using System;
using System.IO;

namespace Orionsoft.MarkdownToPdfLib
{
    public interface IMarkdownToPdf : IDisposable
    {
        ConversionSettings ConversionSettings { get; }
        PageSetup DefaultPageSetup { get; }
        DocumentInfo DocumentInfo { get; }
        Document MigraDocument { get; }
        PluginManager PluginManager { get; }
        StyleManager StyleManager { get; }

        event EventHandler<ConvertingLiteralEventArgs> ConvertingLiteral;

        event EventHandler StylingApplied;

        event EventHandler StylingPrepared;

        event EventHandler<WarningEventArgs> WarningIssued;

        IMarkdownToPdf Add(string markdownText);

        IMarkdownToPdf AddEvenFooter(string footer);

        IMarkdownToPdf AddEvenHeader(string header);

        IMarkdownToPdf AddFirstFooter(string footer);

        IMarkdownToPdf AddFirstHeader(string header);

        IMarkdownToPdf AddFooter(string footer);

        IMarkdownToPdf AddHeader(string header);

        IMarkdownToPdf AddOddFooter(string footer);

        IMarkdownToPdf AddOddHeader(string header);

        IMarkdownToPdf AddSection(bool useDefaultPageSetup = false);

        IMarkdownToPdf Author(string text);

        void Clear();

        IMarkdownToPdf DefalutDpi(double dpi);

        IMarkdownToPdf DefaultFont(string fontName, double fontSize);

        IMarkdownToPdf FirstPageNumber(int pageNumber);

        IMarkdownToPdf FontDir(string fontDir);

        IMarkdownToPdf FooterDistance(Dimension distance);

        IMarkdownToPdf HeaderDistance(Dimension distance);

        IMarkdownToPdf HeadingScale(double scale = 1.125);

        IMarkdownToPdf ImageDir(string imageDir);

        IMarkdownToPdf PageMargins(Dimension left, Dimension right, Dimension top, Dimension bottom);

        IMarkdownToPdf PaperOrientation(PaperOrientation po);

        IMarkdownToPdf PaperSize(PaperSize ps);

        IMarkdownToPdf RegisterLocalFont(string name, string regular, string bold = "", string italic = "", string boldItalic = "");

        void Save(Stream stream);

        void Save(string filename);

        IMarkdownToPdf Title(string text);
    }
}