// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using Orionsoft.MarkdownToPdfLib.Converters;
using Orionsoft.MarkdownToPdfLib.Styling;
using PdfSharp.Fonts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Orionsoft.MarkdownToPdfLib
{
    /// <summary>
    /// <b>Main class of the library</b>
    /// </summary>
    public sealed class MarkdownToPdf : IMarkdownToPdf
    {
        internal readonly DefaultStyles defaultStyles;
        internal readonly List<string> tempFiles = new List<string>();

        #region Properties

        /// <summary>
        /// MigraDoc output document. Can be tweaked directly if needed.
        /// </summary>
        public Document MigraDocument { get; private set; }

        /// <summary>
        /// Additional conversion settings
        /// </summary>
        public ConversionSettings ConversionSettings { get; }

        /// <summary>
        /// PDF Properties e.g. Keywords, Title, etc.
        /// </summary>
        public DocumentInfo DocumentInfo { get => MigraDocument.Info; }

        /// <summary>
        /// Page setup used for new sections
        /// </summary>
        public PageSetup DefaultPageSetup { get; }

        /// <summary>
        /// Enables adding, modifying and binding styles to markdown elements
        /// </summary>
        public StyleManager StyleManager { get; }

        /// <summary>
        /// Enables adding plugins
        /// </summary>
        public PluginManager PluginManager { get; }

        /// <summary>
        /// Calculated PageWidth without margins
        /// </summary>
        internal Unit RealPageWidth { get => GetRealPageWidth(); }

        #endregion Properties

        #region Events

        /// <summary>
        /// Event invoked, when the EvaluatedStyle is ready - i.e. cascading style is combined with it's ancestors, inherited styling is added, as well as attribute styling
        /// </summary>
        public event EventHandler StylingPrepared;

        /// <summary>
        /// Event invoked, after Evaluated style is applied to MigraDoc elements and conversion is about to start
        /// </summary>
        public event EventHandler StylingApplied;

        /// <summary>
        /// Event invoked when non-critical error occurs
        /// </summary>
        public event EventHandler<WarningEventArgs> WarningIssued;

        /// <summary>
        /// Event invoked just before conversion of a literal text span starts. The text can be modified by event handler .
        /// </summary>
        public event EventHandler<ConvertingLiteralEventArgs> ConvertingLiteral;

        #endregion Events

        public MarkdownToPdf()
        {
            ConversionSettings = new ConversionSettings();
            MigraDocument = new Document();
            DefaultPageSetup = MigraDocument.DefaultPageSetup.Clone();
            StyleManager = new StyleManager(this);
            PluginManager = new PluginManager(this);
            GlobalFontSettings.FontResolver = GlobalFontSettings.FontResolver ?? new FontResolver();
            GlobalFontSettings.FontResolver = (GlobalFontSettings.FontResolver as FontResolver) ?? new FontResolver();

            defaultStyles = new DefaultStyles(StyleManager);
            defaultStyles.CreateBasicStyles();
            defaultStyles.BindBasicStyles();
            defaultStyles.InitBasicStyles();
            defaultStyles.InitSmartyPants();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            Clear();
        }

        #region Content Management

        internal IMarkdownToPdf Add(MigraDocBlockContainer container, string markdownText)
        {
            var md = Markdown.Parse(markdownText, ConversionSettings.Pipeline);
            var conv = new RootBlockConvertor(md, markdownText, container, null, this);
            conv.Convert();
            return this;
        }

        /// <summary>
        /// Adds markdown text to body (main document part) of current section. If it does not exist, it is created first;
        /// </summary>
        public IMarkdownToPdf Add(string markdownText)
        {
            var section = GetOrCreateSection();
            var container = new MigraDocBlockContainer(section, this);
            return Add(container, markdownText);
        }

        /// <summary>
        /// Adds new document section
        /// </summary>
        /// <param name="useDefaultPageSetup">if set, DefaultPageSetup is used, otherwise page setup of previous section is copied</param>
        /// <returns></returns>
        public IMarkdownToPdf AddSection(bool useDefaultPageSetup = false)
        {
            var section = MigraDocument.AddSection();

            if (useDefaultPageSetup || MigraDocument.Sections.Count == 1)
            {
                section.PageSetup = DefaultPageSetup.Clone();
            }
            return this;
        }

        #region Headers and footers

        /// <summary>
        /// Adds markdown text to footer of current section. If it does not exist, it is created first;
        /// </summary>
        public IMarkdownToPdf AddFooter(string footer)
        {
            var section = GetOrCreateSection();
            var container = new MigraDocBlockContainer(section.Footers.Primary, this);
            return Add(container, footer);
        }

        /// <summary>
        /// Adds markdown text to footer for even pages of current section. If it does not exist, it is created first;
        /// </summary>
        public IMarkdownToPdf AddEvenFooter(string footer)
        {
            var section = GetOrCreateSection();
            section.PageSetup.OddAndEvenPagesHeaderFooter = true;
            var container = new MigraDocBlockContainer(section.Footers.EvenPage, this);
            return Add(container, footer);
        }

        /// <summary>
        /// Adds markdown text to footer for odd pages of current section. If it does not exist, it is created first;
        /// </summary>
        public IMarkdownToPdf AddOddFooter(string footer)
        {
            var section = GetOrCreateSection();
            section.PageSetup.OddAndEvenPagesHeaderFooter = true;
            return AddFooter(footer);
        }

        /// <summary>
        /// Adds markdown text to footer for first page of current section. If it does not exist, it is created first;
        /// </summary>
        public IMarkdownToPdf AddFirstFooter(string footer)
        {
            var section = GetOrCreateSection();
            var container = new MigraDocBlockContainer(section.Footers.FirstPage, this);
            section.PageSetup.DifferentFirstPageHeaderFooter = true;
            return Add(container, footer);
        }

        /// <summary>
        /// Adds markdown text to footer of current section. If it does not exist, it is created first;
        /// </summary>
        public IMarkdownToPdf AddHeader(string header)
        {
            var section = GetOrCreateSection();
            var container = new MigraDocBlockContainer(section.Headers.Primary, this);
            return Add(container, header);
        }

        /// <summary>
        /// Adds markdown text to Header for even pages of current section. If it does not exist, it is created first;
        /// </summary>
        public IMarkdownToPdf AddEvenHeader(string header)
        {
            var section = GetOrCreateSection();
            section.PageSetup.OddAndEvenPagesHeaderFooter = true;
            var container = new MigraDocBlockContainer(section.Headers.EvenPage, this);
            return Add(container, header);
        }

        /// <summary>
        /// Adds markdown text to Header for odd pages of current section. If it does not exist, it is created first;
        /// </summary>
        public IMarkdownToPdf AddOddHeader(string header)
        {
            var section = GetOrCreateSection();
            section.PageSetup.OddAndEvenPagesHeaderFooter = true;
            return AddHeader(header);
        }

        /// <summary>
        /// Adds markdown text to Header for first page of current section. If it does not exist, it is created first;
        /// </summary>
        public IMarkdownToPdf AddFirstHeader(string header)
        {
            var section = GetOrCreateSection();
            var container = new MigraDocBlockContainer(section.Headers.FirstPage, this);
            section.PageSetup.DifferentFirstPageHeaderFooter = true;
            return Add(container, header);
        }

        #endregion Headers and footers

        /// <summary>
        /// Converts the generated MigraDoc document to PDF and saves it to a file
        /// </summary>
        public void Save(string filename)
        {
            PdfDocumentRenderer pdfRenderer = Render();
            pdfRenderer.PdfDocument.Save(filename);
        }

        /// <summary>
        /// Converts the generated MigraDoc document to PDF and writes it to a stream
        /// </summary>
        public void Save(Stream stream)
        {
            PdfDocumentRenderer pdfRenderer = Render();
            pdfRenderer.PdfDocument.Save(stream);
        }

        /// <summary>
        /// Clears the document contents, but preserves the style definitions and page setup. A new document can be built after calling this method.
        /// </summary>
        public void Clear()
        {
            MigraDocument = new Document();

            foreach (var f in tempFiles.Where(x => File.Exists(x)))
            {
                File.Delete(f);
            }
        }

        #endregion Content Management

        #region Document Info

        /// <summary>
        /// Set document title in PDF properties
        /// </summary>
        public IMarkdownToPdf Title(string text)
        {
            MigraDocument.Info.Title = text;
            return this;
        }

        /// <summary>
        /// Set document author in PDF properties
        /// </summary>
        public IMarkdownToPdf Author(string text)
        {
            MigraDocument.Info.Author = text;
            return this;
        }

        #endregion Document Info

        #region Fluent Styling

        /// <summary>
        /// Sets the default font for the entire generated document. It is always used, unless redefined by an applied style
        /// </summary>
        public IMarkdownToPdf DefaultFont(string fontName, double fontSize)
        {
            defaultStyles.SetDefaultFont(fontName, fontSize);
            return this;
        }

        /// <summary>
        /// By default, H6 header is sized as text and each higher-level heading font size is multiplied by 1.125 (Major second scale).
        /// This method implements easy use of any typographic scale.
        /// E.g. see  <see href="https://type-scale.com/"/>
        /// </summary>
        public IMarkdownToPdf HeadingScale(double scale = 1.125)
        {
            defaultStyles.InitOrUpdateHeadings(scale, fullInit: false);
            return this;
        }

        /// <summary>
        /// This method enables use of local (non-system) fonts in the output
        /// </summary>
        /// <param name="name">Name to be used in styles</param>
        /// <param name="regular">Filename of regular typeface font</param>
        /// <param name="bold">Filename of bold typeface font. If not provided, the typeface is automatically generated from regular</param>
        /// <param name="italic">Filename of italic typeface font. If not provided, the typeface is automatically generated from regular</param>
        /// <param name="boldItalic">Filename of bold and italic typeface font. If not provided, the typeface is automatically generated from other provided fonts</param>
        /// <returns></returns>
        public IMarkdownToPdf RegisterLocalFont(string name, string regular, string bold = "", string italic = "", string boldItalic = "")
        {
            (GlobalFontSettings.FontResolver as FontResolver).Register(name, regular, bold, italic, boldItalic);
            return this;
        }

        /// <summary>
        /// Default path to directory with custom (non-system) fonts
        /// </summary>
        public IMarkdownToPdf FontDir(string fontDir)
        {
            ConversionSettings.FontDir = fontDir;
            return this;
        }

        /// <summary>
        /// Default path to directory with images referenced from markdown text
        /// </summary>
        public IMarkdownToPdf ImageDir(string imageDir)
        {
            ConversionSettings.ImageDir = imageDir;
            return this;
        }

        /// <summary>
        /// If set, all images without explicit DPI settings are added with DPI set to this value
        /// </summary>
        public IMarkdownToPdf DefalutDpi(double dpi)
        {
            ConversionSettings.DefaultDpi = dpi;
            return this;
        }

        /// <summary>
        /// Sets PDF page size for current section if exists, otherwise for <see cref="DefaultPageSetup"/>
        /// </summary>
        public IMarkdownToPdf PaperSize(PaperSize ps)
        {
            PageSetup target = GetCurrentPageSetup();
            var sizes = new Dictionary<PaperSize, (double Width, double Height)>()
            {
                { MarkdownToPdfLib.PaperSize.A0, (841, 1189)},
                { MarkdownToPdfLib.PaperSize.A1, (594, 841)},
                { MarkdownToPdfLib.PaperSize.A2, (420, 594)},
                { MarkdownToPdfLib.PaperSize.A3, (297, 420)},
                { MarkdownToPdfLib.PaperSize.A4, (210, 297)},
                { MarkdownToPdfLib.PaperSize.A5, (148, 210)},
                { MarkdownToPdfLib.PaperSize.A6, (105, 148)},
                { MarkdownToPdfLib.PaperSize.B0, (1000, 1414)},
                { MarkdownToPdfLib.PaperSize.B1, (707, 1000)},
                { MarkdownToPdfLib.PaperSize.B2, (500, 707)},
                { MarkdownToPdfLib.PaperSize.B3, (353, 500)},
                { MarkdownToPdfLib.PaperSize.B4, (250, 353)},
                { MarkdownToPdfLib.PaperSize.B5, (176, 250)},
                { MarkdownToPdfLib.PaperSize.B6, (125, 176)},
                { MarkdownToPdfLib.PaperSize.Letter, (216, 279)},
                { MarkdownToPdfLib.PaperSize.Legal, (216, 356)},
                { MarkdownToPdfLib.PaperSize.Ledger, (279, 432)},
                { MarkdownToPdfLib.PaperSize.Tabloid, (279, 432)},
                { MarkdownToPdfLib.PaperSize.P11x17, (11 * 25.4, 17 * 25.4)}
            };
            target.PageWidth = Unit.FromMillimeter(sizes[ps].Width);
            target.PageHeight = Unit.FromMillimeter(sizes[ps].Height);

            if (ps < MarkdownToPdfLib.PaperSize.B0)
                target.PageFormat = (PageFormat)ps;
            return this;
        }

        /// <summary>
        /// Sets PDF page orientation for current section if exists, otherwise for <see cref="DefaultPageSetup"/>
        /// </summary>
        public IMarkdownToPdf PaperOrientation(PaperOrientation po)
        {
            PageSetup target = GetCurrentPageSetup();
            target.Orientation = (Orientation)po;
            return this;
        }

        /// <summary>
        /// Sets PDF page margins for current section if exists, otherwise for <see cref="DefaultPageSetup"/>
        /// </summary>
        public IMarkdownToPdf PageMargins(Dimension left, Dimension right, Dimension top, Dimension bottom)
        {
            PageSetup target = GetCurrentPageSetup();
            target.LeftMargin = left.Eval(defaultStyles.fontSize, RealPageWidth);
            target.RightMargin = right.Eval(defaultStyles.fontSize, RealPageWidth);
            target.TopMargin = top.Eval(defaultStyles.fontSize, RealPageWidth);
            target.BottomMargin = bottom.Eval(defaultStyles.fontSize, RealPageWidth);
            return this;
        }

        /// <summary>
        /// Sets PDF page header distance from physical page border for current section if exists, otherwise for <see cref="DefaultPageSetup"/>
        /// </summary>
        public IMarkdownToPdf HeaderDistance(Dimension distance)
        {
            PageSetup target = GetCurrentPageSetup();
            target.HeaderDistance = distance.Eval(defaultStyles.fontSize, RealPageWidth);
            return this;
        }

        /// <summary>
        /// Sets PDF page footer distance from physical page border for current section if exists, otherwise for <see cref="DefaultPageSetup"/>
        /// </summary>
        public IMarkdownToPdf FooterDistance(Dimension distance)
        {
            PageSetup target = GetCurrentPageSetup();
            target.FooterDistance = distance.Eval(defaultStyles.fontSize, RealPageWidth);
            return this;
        }

        /// <summary>
        /// Sets starting page number for current section if exists, otherwise for <see cref="DefaultPageSetup"/>
        /// If it is not set, the section numbering continues from previous section
        /// </summary>
        public IMarkdownToPdf FirstPageNumber(int pageNumber)
        {
            PageSetup target = GetCurrentPageSetup();
            target.StartingNumber = pageNumber;
            return this;
        }

        #endregion Fluent Styling

        #region Event Invokers

        internal void OnStylingPrepared(object o) => StylingPrepared?.Invoke(o, EventArgs.Empty);

        internal void OnStylingApplied(object o) => StylingApplied?.Invoke(o, EventArgs.Empty);

        internal string OnConvertingLiteral(object o, string text)
        {
            var args = new ConvertingLiteralEventArgs { Text = text };
            ConvertingLiteral?.Invoke(o, args);
            return args.Text;
        }

        internal void OnWarningIssued(object o, string category, string message) =>
            WarningIssued?.Invoke(o, new WarningEventArgs { Category = category, Message = message });

        #endregion Event Invokers

        #region Private methods

        private PageSetup GetCurrentPageSetup()
        {
            PageSetup target;

            if (MigraDocument.LastSection == null)
            {
                target = DefaultPageSetup;
            }
            else
            {
                target = MigraDocument.LastSection.PageSetup;
            }

            return target;
        }

        private Section GetOrCreateSection()
        {
            if (MigraDocument.LastSection == null)
            {
                var section = MigraDocument.AddSection();
                section.PageSetup = DefaultPageSetup.Clone();
            }
            return MigraDocument.LastSection;
        }

        private Unit GetRealPageWidth()
        {
            PageSetup target = GetCurrentPageSetup();
            var width = target.PageWidth;
            width = width.IsEmpty ? DefaultPageSetup.PageWidth : width;

            var lm = target.LeftMargin;
            lm = lm.IsEmpty ? DefaultPageSetup.LeftMargin : lm;
            var rm = target.RightMargin;
            rm = rm.IsEmpty ? DefaultPageSetup.RightMargin : rm;

            var res = width - lm - rm;

            if (res <= 0) throw new InvalidOperationException("Invalid page size");
            return res;
        }

        private PdfDocumentRenderer Render()
        {
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true)
            {
                Document = MigraDocument
            };

            pdfRenderer.RenderDocument();
            return pdfRenderer;
        }

        #endregion Private methods
    }
}