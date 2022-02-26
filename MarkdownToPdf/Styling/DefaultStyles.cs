// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using MigraDoc.DocumentObjectModel;
using Orionsoft.MarkdownToPdfLib.Styling;
using System;

namespace Orionsoft.MarkdownToPdfLib
{
    internal class DefaultStyles
    {
        private readonly StyleManager styleManager;
        internal double fontSize;
        private double headingScale;
        internal string fontName;
        private Color codeBg = Color.FromRgb(240, 240, 240);
        private readonly double defaultIndent;

        internal DefaultStyles(StyleManager styleManager)
        {
            this.styleManager = styleManager;
            fontName = "Arial";
            fontSize = 11;
            defaultIndent = 2;
            headingScale = 1.125;
        }

        internal void CreateBasicStyles()
        {
            styleManager.AddStyle(MarkdownStyleNames.Undefined);

            // Containers

            styleManager.AddStyle(MarkdownStyleNames.Root);
            styleManager.AddStyle(MarkdownStyleNames.UnorderedList);
            styleManager.AddStyle(MarkdownStyleNames.UnorderedListItem);
            styleManager.AddStyle(MarkdownStyleNames.OrderedList, MarkdownStyleNames.UnorderedList);
            styleManager.AddStyle(MarkdownStyleNames.OrderedListItem);
            styleManager.AddStyle(MarkdownStyleNames.Quote);

            styleManager.AddStyle(MarkdownStyleNames.FootnoteGroup);
            styleManager.AddStyle(MarkdownStyleNames.Footnote);
            styleManager.AddStyle(MarkdownStyleNames.Table);
            styleManager.AddStyle(MarkdownStyleNames.TableHeader);
            styleManager.AddStyle(MarkdownStyleNames.TableRowOdd);
            styleManager.AddStyle(MarkdownStyleNames.TableRowEven);
            styleManager.AddStyle(MarkdownStyleNames.TableCell);
            styleManager.AddStyle(MarkdownStyleNames.CustomContainer);

            // Leafblocks

            styleManager.AddStyle(MarkdownStyleNames.Paragraph);
            styleManager.AddStyle(MarkdownStyleNames.Code);
            styleManager.AddStyle(MarkdownStyleNames.Break);

            styleManager.AddStyle(MarkdownStyleNames.QuoteParagraph, MarkdownStyleNames.Paragraph);
            styleManager.AddStyle(MarkdownStyleNames.FootnoteParagraph, MarkdownStyleNames.Paragraph);

            styleManager.AddStyle(MarkdownStyleNames.Image);
            styleManager.AddStyle(MarkdownStyleNames.Plugin);

            // Inlines

            styleManager.AddStyle(MarkdownStyleNames.Bold);
            styleManager.AddStyle(MarkdownStyleNames.Italic);
            styleManager.AddStyle(MarkdownStyleNames.Hyperlink);
            styleManager.AddStyle(MarkdownStyleNames.InlineCode);

            styleManager.AddStyle(MarkdownStyleNames.FootnoteReference);
            styleManager.AddStyle(MarkdownStyleNames.Subscript);
            styleManager.AddStyle(MarkdownStyleNames.Superscript);
            styleManager.AddStyle(MarkdownStyleNames.Cite);
            styleManager.AddStyle(MarkdownStyleNames.Marked);
            styleManager.AddStyle(MarkdownStyleNames.Inserted);
            styleManager.AddStyle(MarkdownStyleNames.Strike);
            styleManager.AddStyle(MarkdownStyleNames.Index);

            styleManager.AddStyle(MarkdownStyleNames.InlineImage);
            styleManager.AddStyle(MarkdownStyleNames.InlinePlugin);

            CreateAndBindHeadings();
        }

        internal void BindBasicStyles()
        {
            // Containers

            styleManager.ForElement(ElementType.Root).Bind(MarkdownStyleNames.Root);
            styleManager.ForElement(ElementType.UnorderedList).Bind(MarkdownStyleNames.UnorderedList);
            styleManager.ForElement(ElementType.OrderedList).Bind(MarkdownStyleNames.UnorderedList);
            styleManager.ForElement(ElementType.OrderedListItem).Bind(MarkdownStyleNames.OrderedListItem);
            styleManager.ForElement(ElementType.UnorderedListItem).Bind(MarkdownStyleNames.UnorderedListItem);
            styleManager.ForElement(ElementType.Quote).Bind(MarkdownStyleNames.Quote);

            styleManager.ForElement(ElementType.FootnoteGroup).Bind(MarkdownStyleNames.FootnoteGroup);
            styleManager.ForElement(ElementType.Footnote).Bind(MarkdownStyleNames.Footnote);
            styleManager.ForElement(ElementType.Table).Bind(MarkdownStyleNames.Table);
            styleManager.ForElement(ElementType.TableHeader).Bind(MarkdownStyleNames.TableHeader);
            styleManager.ForElement(ElementType.TableRowEven).Bind(MarkdownStyleNames.TableRowEven);
            styleManager.ForElement(ElementType.TableRowOdd).Bind(MarkdownStyleNames.TableRowOdd);
            styleManager.ForElement(ElementType.TableCell).Bind(MarkdownStyleNames.TableCell);

            styleManager.ForElement(ElementType.CustomContainer).Bind(MarkdownStyleNames.CustomContainer);

            // Leafblocks

            styleManager.ForElement(ElementType.Paragraph).Bind(MarkdownStyleNames.Paragraph);
            styleManager.ForElement(ElementType.Break).Bind(MarkdownStyleNames.Break);
            styleManager.ForElement(ElementType.Code).Bind(MarkdownStyleNames.Code);
            styleManager.ForElement(ElementType.Image).Bind(MarkdownStyleNames.Image);
            styleManager.ForElement(ElementType.Plugin).Bind(MarkdownStyleNames.Plugin);
            styleManager.ForElement(ElementType.Paragraph).WithAncestor(ElementType.Quote).Bind(MarkdownStyleNames.QuoteParagraph);
            styleManager.ForElement(ElementType.Paragraph).WithParent(ElementType.Footnote).Bind(MarkdownStyleNames.FootnoteParagraph);

            // Inlines

            styleManager.ForElement(ElementType.Bold).Bind(MarkdownStyleNames.Bold);
            styleManager.ForElement(ElementType.Italic).Bind(MarkdownStyleNames.Italic);
            styleManager.ForElement(ElementType.Hyperlink).Bind(MarkdownStyleNames.Hyperlink);
            styleManager.ForElement(ElementType.InlineCode).Bind(MarkdownStyleNames.InlineCode);

            styleManager.ForElement(ElementType.FootnoteReference).Bind(MarkdownStyleNames.FootnoteReference);
            styleManager.ForElement(ElementType.Subscript).Bind(MarkdownStyleNames.Subscript);
            styleManager.ForElement(ElementType.Superscript).Bind(MarkdownStyleNames.Superscript);
            styleManager.ForElement(ElementType.Cite).Bind(MarkdownStyleNames.Cite);
            styleManager.ForElement(ElementType.Marked).Bind(MarkdownStyleNames.Marked);
            styleManager.ForElement(ElementType.Inserted).Bind(MarkdownStyleNames.Inserted);
            styleManager.ForElement(ElementType.Strike).Bind(MarkdownStyleNames.Strike);
            styleManager.ForElement(ElementType.Index).Bind(MarkdownStyleNames.Index);
        }

        internal void InitBasicStyles(bool fullInit = true)
        {
            CascadingStyle style;

            style = styleManager.Styles[MarkdownStyleNames.Undefined];
            if (fullInit)
            {
                style.Font.Color = Colors.Green;
                style.Font.Underline = Underline.Dotted;
            }

            InitContainers();
            InitLeafBlocks();
            InitInlines();
            InitOrUpdateHeadings(headingScale, fullInit: true);
        }

        internal void InitOrUpdateHeadings(double scale, bool fullInit)
        {
            headingScale = scale;
            for (int i = 1; i <= 6; i++)
            {
                var style = styleManager.Styles["Heading" + i];
                if (fullInit)
                {
                    style.Font.Bold = true;
                    style.Paragraph.KeepWithNext = true;
                    style.Paragraph.OutlineLevel = OutlineLevel.BodyText + i;

                    var tocStyle = styleManager.Styles["Toc" + i];
                    tocStyle.Font.Color = Colors.Black;
                    tocStyle.Margin.Left = Dimension.FromFontSize(defaultIndent * (i - 1));
                }

                var fs = Dimension.FromFontSize(Math.Pow(scale, 6 - i));
                style.Font.Size = fs;
                style.Margin.Top = ".8em";
                style.Margin.Bottom = ".5em";
            }
        }

        internal void InitSmartyPants()
        {
            //     This is a left single quote ' -gt; lsquo; x8216
            styleManager.Owner.ConversionSettings.SmartyPantsMapping[Markdig.Extensions.SmartyPants.SmartyPantType.LeftQuote] = "‘";
            //
            // Summary:
            //     This is a right single quote ' -gt; rsquo; x8217
            styleManager.Owner.ConversionSettings.SmartyPantsMapping[Markdig.Extensions.SmartyPants.SmartyPantType.RightQuote] = "’";
            //
            //
            // Summary:
            //     This is a left double quote " -gt; ldquo;x8220
            styleManager.Owner.ConversionSettings.SmartyPantsMapping[Markdig.Extensions.SmartyPants.SmartyPantType.LeftDoubleQuote] = "“";
            //
            // Summary:
            //     This is a right double quote " -gt; rdquo; x8221
            styleManager.Owner.ConversionSettings.SmartyPantsMapping[Markdig.Extensions.SmartyPants.SmartyPantType.RightDoubleQuote] = "”";
            //
            // Summary:
            //     This is a right double quote << -gt; laquo; x171
            styleManager.Owner.ConversionSettings.SmartyPantsMapping[Markdig.Extensions.SmartyPants.SmartyPantType.LeftAngleQuote] = "«";
            //
            // Summary:
            //     This is a right angle quote >> -gt; raquo; x187
            styleManager.Owner.ConversionSettings.SmartyPantsMapping[Markdig.Extensions.SmartyPants.SmartyPantType.RightAngleQuote] = "»";
            //
            // Summary:
            //     This is an ellipsis ... -gt; hellip; \x8230
            styleManager.Owner.ConversionSettings.SmartyPantsMapping[Markdig.Extensions.SmartyPants.SmartyPantType.Ellipsis] = "…";
            //
            // Summary:
            //     This is a ndash -- -gt; ndash; \x8211
            styleManager.Owner.ConversionSettings.SmartyPantsMapping[Markdig.Extensions.SmartyPants.SmartyPantType.Dash2] = "–";
            //
            // Summary:
            //     This is a mdash --- -gt; mdash; \x8212
            styleManager.Owner.ConversionSettings.SmartyPantsMapping[Markdig.Extensions.SmartyPants.SmartyPantType.Dash3] = "—";
        }

        internal void SetDefaultFont(string fontName, double fontSize)
        {
            this.fontName = fontName;
            this.fontSize = fontSize;

            var root = styleManager.Styles[MarkdownStyleNames.Root];
            root.Font.Name = fontName;
            root.Font.Size = fontSize;
        }

        private void InitContainers()
        {
            CascadingStyle style;

            style = styleManager.Styles[MarkdownStyleNames.Root];
            style.Font.Name = fontName;
            style.Font.Size = fontSize;

            style = styleManager.Styles[MarkdownStyleNames.UnorderedList];
            style.Margin.Left = Dimension.FromFontSize(defaultIndent);
            style.Margin.Top = ".75em";
            style.Margin.Bottom = ".75em";

            style = styleManager.Styles[MarkdownStyleNames.UnorderedListItem];
            style.Bullet.Normal.Content = "\x2022";
            style.Bullet.Unchecked.Font.Name = "Wingdings";
            style.Bullet.Unchecked.Content = "\xA8";
            style.Bullet.Checked.Font.Name = "Wingdings";
            style.Bullet.Checked.Content = "\xFE";
            style.Bullet.Normal.Font.Bold = false;
            style.Bullet.Normal.Font.Italic = false;
            style.Bullet.Normal.Font.Superscript = false;
            style.Bullet.Normal.Font.Subscript = false;
            style.Bullet.Checked.Font.Bold = false;
            style.Bullet.Checked.Font.Italic = false;
            style.Bullet.Checked.Font.Superscript = false;
            style.Bullet.Checked.Font.Subscript = false;
            style.Bullet.Unchecked.Font.Bold = false;
            style.Bullet.Unchecked.Font.Italic = false;
            style.Bullet.Unchecked.Font.Superscript = false;
            style.Bullet.Unchecked.Font.Subscript = false;
            style.Bullet.TextIndent = Dimension.FromFontSize(defaultIndent);
            style.Bullet.BulletIndent = Dimension.FromFontSize(defaultIndent * 0.5);
            style.Margin.Top = ".5em";

            // Currently no other styling for Ordered list then Unordered: style = styleManager.Styles[MarkdownStyleNames.OrderedList]

            style = styleManager.Styles[MarkdownStyleNames.OrderedListItem];
            style.Bullet.TextIndent = Dimension.FromFontSize(defaultIndent);
            style.Margin.Top = ".5em";
            style.Bullet.Normal.Content = ".";
            style.Bullet.BulletIndent = Dimension.FromFontSize(defaultIndent * 0.75);

            style = styleManager.Styles[MarkdownStyleNames.Quote];
            style.Font.Color = Color.FromRgb(80, 80, 80);
            style.Background = Colors.White;
            style.Margin.Top = "1em";
            style.Margin.Bottom = "1em";
            style.Padding.Top = "1em";
            style.Padding.Bottom = ".5em";
            style.Margin.Left = Dimension.FromFontSize(defaultIndent);

            style = styleManager.Styles[MarkdownStyleNames.Footnote];
            style.Bullet.Normal.Content = ".";
            style.Bullet.TextIndent = Dimension.FromFontSize(defaultIndent);
            style.Bullet.BulletIndent = Dimension.FromFontSize(defaultIndent * 0.8);

            style = styleManager.Styles[MarkdownStyleNames.FootnoteGroup];
            style.Margin.Top = Unit.FromPoint(fontSize);

            style = styleManager.Styles[MarkdownStyleNames.Table];
            style.Margin.Top = "1em";
            style.Margin.Bottom = "1em";
            style.Table.CellSpacing = ".5em";
            style.Table.CellSpacing.Bottom = "0em";
            style.Border.Width = .8;
            style.Border.Color = Colors.Gray;

            style = styleManager.Styles[MarkdownStyleNames.TableHeader];
            style.Font.Bold = true;
            style.Border.Bottom.Width = .8;
            style.Border.Bottom.Color = Colors.Gray;

            style = styleManager.Styles[MarkdownStyleNames.TableCell];
            style.Border.Width = .4;
            style.Border.Color = Colors.Gray;

            style.Border.Bottom.Width = new Dimension();
            style.Border.Bottom.Color = Color.Empty;

            style = styleManager.Styles[MarkdownStyleNames.CustomContainer];
            style.Background = Colors.AliceBlue;
            style.Padding.Left = "1em";
            style.Padding.Right = "1em";
            style.Padding.Top = "1em";
            style.Padding.Bottom = "1em";
            style.Margin.Top = "1em";
            style.Margin.Bottom = "1em";
        }

        private void InitInlines()
        {
            CascadingStyle style;
            style = styleManager.Styles[MarkdownStyleNames.Bold];
            style.Font.Bold = true;

            style = styleManager.Styles[MarkdownStyleNames.Italic];
            style.Font.Italic = true;

            style = styleManager.Styles[MarkdownStyleNames.Hyperlink];
            style.Font.Color = Colors.Blue;

            style = styleManager.Styles[MarkdownStyleNames.InlineCode];
            style.Font.Name = "Consolas";
            style.Font.Color = Colors.Chocolate;

            style = styleManager.Styles[MarkdownStyleNames.FootnoteReference];
            style.Font.Superscript = true;

            style = styleManager.Styles[MarkdownStyleNames.Superscript];
            style.Font.Superscript = true;

            style = styleManager.Styles[MarkdownStyleNames.Subscript];
            style.Font.Subscript = true;

            style = styleManager.Styles[MarkdownStyleNames.Cite];
            style.Font.Underline = Underline.Dotted;

            style = styleManager.Styles[MarkdownStyleNames.Marked];
            style.Font.Color = Colors.Red;
            style.Font.Bold = true;

            style = styleManager.Styles[MarkdownStyleNames.Inserted];
            style.Font.Color = Colors.Green;

            style = styleManager.Styles[MarkdownStyleNames.Strike];
            style.Font.Color = Colors.Gray;
            style.Font.Italic = true;

            // Special inlines

            style = styleManager.Styles[MarkdownStyleNames.Index];
            style.Font.Color = Colors.Black;
        }

        private void InitLeafBlocks()
        {
            CascadingStyle style;
            style = styleManager.Styles[MarkdownStyleNames.Paragraph];
            style.Paragraph.WidowControl = true;
            style.Margin.Bottom = ".75em";

            style = styleManager.Styles[MarkdownStyleNames.Break];
            style.Border.Bottom.Color = Colors.Gray;
            style.Border.Bottom.Width = .25;
            style.Margin.Bottom = "1.5em";

            style = styleManager.Styles[MarkdownStyleNames.QuoteParagraph];
            style.Border.Left.LineStyle = MigraDoc.DocumentObjectModel.BorderStyle.Single;
            style.Border.Left.Color = Colors.LightGray;
            style.Padding.Left = Dimension.FromFontSize(defaultIndent * 0.5);
            style.Padding.Bottom = style.Margin.Bottom;
            style.Border.Left.Width = ".25em";
            style.Margin.Bottom = 0;

            style = styleManager.Styles[MarkdownStyleNames.Code];
            style.Font.Name = "Consolas";
            style.Font.Size = "1em";
            style.Background = codeBg;
            style.Margin.Bottom = fontSize;
            style.Margin.Top = ".5em";
            style.Padding.Bottom = ".5em";
            style.Padding.Left = ".5em";
            style.Padding.Top = ".5em";
            style.Padding.Right = ".5em";

            style = styleManager.Styles[MarkdownStyleNames.Image];

            style.Paragraph.Alignment = ParagraphAlignment.Center;
            style.Margin.Top = "1em";
            style.Margin.Bottom = "1em";

            style = styleManager.Styles[MarkdownStyleNames.Plugin];

            style.Paragraph.Alignment = ParagraphAlignment.Center;
            style.Margin.Top = "1em";
            style.Margin.Bottom = "1em";
        }

        private void CreateAndBindHeadings()
        {
            for (int i = 1; i <= 6; i++)
            {
                var style = styleManager.AddStyle("Heading" + i);
                styleManager.ForElement(ElementType.Heading1 + i - 1).Bind(style);

                style = styleManager.AddStyle("Toc" + i);
                styleManager.ForElement(ElementType.Toc1 + i - 1).Bind(style);
            }
        }
    }
}