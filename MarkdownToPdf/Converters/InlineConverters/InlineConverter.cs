// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig.Extensions.Footnotes;
using Markdig.Extensions.Mathematics;
using Markdig.Extensions.SmartyPants;
using Markdig.Extensions.TaskLists;
using Markdig.Syntax.Inlines;
using MigraDoc.DocumentObjectModel;
using Orionsoft.MarkdownToPdfLib.Styling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Orionsoft.MarkdownToPdfLib.Converters
{
    internal class InlineConverter : IInlineConverter
    {
        private FontStyle formatting;

        public Inline Inline { get; protected set; }
        public MigrDocInlineContainer OutputParagraph { get; protected set; }

        public IBlockConverter Parent { get; protected set; }
        public InlineConverter ParentInlineConverter { get; protected set; }
        IInlineConverter IInlineConverter.ParentInlineConverter { get => ParentInlineConverter; }

        public ElementAttributes Attributes { get; }
        public SingleElementDescriptor ElementDescriptor { get; protected set; }
        public StylingDescriptor Descriptor => new StylingDescriptor(GetDescriptors());

        public InlineConverter(Inline inline, InlineConverter Parent, IBlockConverter ParentBlock,
            MigrDocInlineContainer OutputParagraph, FontStyle formatting = null)
        {
            formatting = formatting ?? new FontStyle();
            this.Inline = inline;
            this.formatting = formatting;
            this.OutputParagraph = OutputParagraph;
            this.Parent = ParentBlock;
            this.ParentInlineConverter = Parent;

            Attributes = new ElementAttributes(GetTextAfter());

            if (inline is EmphasisInline)
            {
                Attributes.Markup = (inline as EmphasisInline).DelimiterChar.ToString();
            }

            ElementDescriptor = new SingleElementDescriptor { Attributes = Attributes, PlainText = GetInlinePlainText(), Position = new ElementPosition(Inline) };
        }

        internal bool Convert()
        {
            if (Inline is LiteralInline)
            {
                return AddLiteral();
            }

            if (Inline is EmphasisInline)
            {
                return AddEmphasis();
            }

            if (Inline is LineBreakInline)
            {
                AddLineBreak();
                return true;
            }

            if (Inline is CodeInline)
            {
                AddInlineCode();
                return true;
            }

            if (Inline is HtmlEntityInline)
            {
                var he = Inline as HtmlEntityInline;
                AddTextSpan(he.Transcoded.ToString(), formatting);
                return true;
            }

            if (Inline is LinkInline)
            {
                var lnk = Inline as LinkInline;
                if (lnk.IsImage)
                {
                    AddInlineImage(lnk);
                }
                else
                {
                    AddInlineLink(lnk, formatting);
                }
                return true;
            }

            if (Inline is FootnoteLink)
            {
                return AddFootnoteLink();
            }

            if (Inline is SmartyPant)
            {
                return AddSmartyPant();
            }

            if (Inline is HtmlInline)
            {
                return AddHtmlInline();
            }

            if (Inline is TaskList)
            {
                // fixes parser result returning extra space
                if (Inline.NextSibling is LiteralInline ns && ns.Content.ToString().StartsWith(" "))
                    ns.Content.Start++;
                return true;
            }

            if (Inline is MathInline)
            {
                return AddMath();
            }
            Parent.Owner.OnWarningIssued(this, "Inline", "Unsupported inline " + Inline.GetType().Name + $", line { Inline.Line}");
            return false;
        }

        private bool AddLiteral()
        {
            if (Inline.PreviousSibling is SmartyPant)
            {
                var text = (Inline as LiteralInline).Content.ToString();
                AppendPlainText(text);
            }
            else
            {
                AddTextSpan((Inline as LiteralInline).Content.ToString(), formatting);
            }
            return true;
        }

        private void AddLineBreak()
        {
            if ((Inline as LineBreakInline).IsHard)
            {
                OutputParagraph.AddLineBreak();
            }
            else
            {
                OutputParagraph.AddText(" ");
            }
        }

        private bool AddSmartyPant()
        {
            var sp = Inline as SmartyPant;
            var owner = Parent?.Owner;
            string text;

            if (owner != null)
            {
                text = owner.ConversionSettings.SmartyPantsMapping.ContainsKey(sp.Type) ? owner.ConversionSettings.SmartyPantsMapping[sp.Type] : sp.AsLiteralInline().Content.ToString();
            }
            else
            {
                text = sp.AsLiteralInline().Content.ToString();
            }

            if (Inline.PreviousSibling is LiteralInline)
            {
                AppendPlainText(text);
            }
            else
            {
                OutputParagraph.AddText(text);
            }

            return true;
        }

        private void AppendPlainText(string text)
        {
            var lo = OutputParagraph.Paragraph.Elements?.LastObject;
            while (lo is FormattedText)
            {
                lo = (lo as FormattedText).Elements.LastObject;
            }
            if (lo is Text)
            {
                (lo as Text).Content += text;
            }
            else
            {
                Parent.Owner.OnWarningIssued(this, "Bug", "AppendPlainText failed");
            }
        }

        private bool AddEmphasis()
        {
            formatting = formatting.Clone();
            var emph = Inline as EmphasisInline;

            if ((emph.DelimiterChar == '*' || emph.DelimiterChar == '_') && emph.DelimiterCount == 2) return AddExtendedEmphasis(ElementType.Bold);
            if ((emph.DelimiterChar == '*' || emph.DelimiterChar == '_') && emph.DelimiterCount == 1) return AddExtendedEmphasis(ElementType.Italic);
            if (emph.DelimiterChar == '^' && emph.DelimiterCount == 1) return AddExtendedEmphasis(ElementType.Superscript);
            if (emph.DelimiterChar == '~' && emph.DelimiterCount == 1) return AddExtendedEmphasis(ElementType.Subscript);

            if (emph.DelimiterChar == '"' && emph.DelimiterCount == 2) return AddExtendedEmphasis(ElementType.Cite);
            if (emph.DelimiterChar == '~' && emph.DelimiterCount == 2) return AddExtendedEmphasis(ElementType.Strike);
            if (emph.DelimiterChar == '+' && emph.DelimiterCount == 2) return AddExtendedEmphasis(ElementType.Inserted);
            if (emph.DelimiterChar == '=' && emph.DelimiterCount == 2) return AddExtendedEmphasis(ElementType.Marked);

            foreach (var emphInline in emph)
            {
                var c = new InlineConverter(emphInline, this, Parent, OutputParagraph, formatting);
                c.Convert();
            }

            return true;
        }

        private bool AddExtendedEmphasis(ElementType type)
        {
            var emph = Inline as EmphasisInline;
            ElementDescriptor.Type = type;
            var style = GetStyle().Eval();
            formatting = style.Font.MergeWith(formatting);

            foreach (var item in emph)
            {
                var c = new InlineConverter(item, this, Parent, OutputParagraph, formatting);
                c.Convert();
            }

            return true;
        }

        private void AddInlineCode()
        {
            ElementDescriptor.Type = ElementType.InlineCode;
            var style = GetStyle().Eval();
            formatting = style.Font.MergeWith(formatting);
            var code = Inline as CodeInline;

            var res = Parent.Owner.PluginManager.Highlight(new List<string> { code.Content }, this);
            if (res?.Success ?? false)
            {
                foreach (var s in res.Spans)
                {
                    formatting.Color = Color.FromArgb(255, s.Color.R, s.Color.G, s.Color.B);
                    formatting.Bold = s.Bold ? true : formatting.Bold;
                    formatting.Italic = s.Italic ? true : formatting.Italic;
                    formatting.Underline = s.Underline ? Underline.Single : formatting.Underline;

                    AddTextSpan(s.Text, formatting);
                }
            }
            else
            {
                AddTextSpan(code.Content, formatting);
            }
        }

        private bool AddFootnoteLink()
        {
            var footnotelink = Inline as FootnoteLink;
            if (footnotelink.IsBackLink) return true;  // back links are ignored
            ElementDescriptor.Type = ElementType.FootnoteReference;
            var style = GetStyle().Eval();
            formatting = style.Font.MergeWith(formatting);

            var idx = footnotelink.Footnote.Parent.IndexOf(footnotelink.Footnote) + 1;
            var hyper = OutputParagraph.AddHyperlink("Footnote_" + idx, HyperlinkType.Local);
            var font = new Font();

            hyper.AddFormattedText(idx.ToString(), formatting.MergeWithFont(font, Parent.FontSize, Parent.Width, alreadyScaled: false));
            return true;
        }

        private void AddInlineLink(LinkInline lnk, FontStyle formatting)
        {
            ElementDescriptor.Type = ElementType.Hyperlink;

            if (lnk.Url != null)
            {
                if (lnk.Url.StartsWith("md:"))
                {
                    ExecuteInlineField(lnk);
                    return;
                }

                var hyper = OutputParagraph.AddHyperlink(lnk.Url.TrimStart('#'),
                    lnk.Url.StartsWith("#") ? HyperlinkType.Local : HyperlinkType.Url);

                var toc = Attributes.Style.HasValue() && Regex.Match(Attributes.Style, "^(Toc[1-6]|Index)$").Success;

                if (toc)
                {
                    switch (Attributes.Style)
                    {
                        case "Toc1": ElementDescriptor.Type = ElementType.Toc1; break;
                        case "Toc2": ElementDescriptor.Type = ElementType.Toc2; break;
                        case "Toc3": ElementDescriptor.Type = ElementType.Toc3; break;
                        case "Toc4": ElementDescriptor.Type = ElementType.Toc4; break;
                        case "Toc5": ElementDescriptor.Type = ElementType.Toc5; break;
                        case "Toc6": ElementDescriptor.Type = ElementType.Toc6; break;
                        case "Index": ElementDescriptor.Type = ElementType.Index; break;
                    }
                }
                var style = GetStyle().Eval();

                hyper.Font.ApplyFont(style.Font.MergeWithFont(hyper.Font, Parent.FontSize, Parent.Width, alreadyScaled: false));

                foreach (var item in lnk)
                {
                    var c = new InlineConverter(item, this, Parent, hyper, formatting);
                    c.Convert();
                }

                if (toc)
                {
                    OutputParagraph.Format = style.Merge(OutputParagraph.Format, Parent.FontSize, Parent.Width);
                    OutputParagraph.Format.LeftIndent += style.Margin.Left.Eval(Parent.FontSize, Parent.Width);
                    var w = Parent.Owner.RealPageWidth;
                    OutputParagraph.Paragraph.Format.AddTabStop(w, TabAlignment.Right, TabLeader.Dots);

                    hyper.AddFormattedText("\t", hyper.Font);
                    hyper.AddPageRefField(lnk.Url.TrimStart('#'));
                }
            }
        }

        private void AddInlineImage(LinkInline lnk)
        {
            if (lnk.Url.StartsWith("md:plugin"))
            {
                AddPlugin();
                return;
            }

            var path = Parent.Owner.ConversionSettings.ImageDir;
            if (path.HasValue() && !(path.EndsWith(Path.DirectorySeparatorChar.ToString()) || path.EndsWith(Path.AltDirectorySeparatorChar.ToString())))
            {
                path += Path.DirectorySeparatorChar;
            }
            path += lnk.Url;
            var img = OutputParagraph.AddImage(path);

            if (Parent.Owner.ConversionSettings.DefaultDpi > 0 && img.Resolution == 0) img.Resolution = Parent.Owner.ConversionSettings.DefaultDpi;
            var width = Attributes.ContainsKey("width") ? Attributes["width"] : "";
            var height = Attributes.ContainsKey("height") ? Attributes["height"] : "";
            var dpi = Attributes["dpi"];
            if (dpi != null)
            {
                try
                {
                    img.Resolution = double.Parse(dpi);
                }
                catch (FormatException e)
                {
                    Parent.Owner.OnWarningIssued(this, "DPI", e.Message + $", line { lnk.Line}");
                }
            }
            try
            {
                if (height.HasValue()) img.Height = Dimension.Parse(height).Eval(Parent.FontSize, Parent.Width);
            }
            catch (ArgumentException e)
            {
                Parent.Owner.OnWarningIssued(this, "Dimension", e.Message + $", line { lnk.Line}");
            }

            try
            {
                if (width.HasValue()) img.Width = Dimension.Parse(width).Eval(Parent.FontSize, Parent.Width);
            }
            catch (ArgumentException e)
            {
                Parent.Owner.OnWarningIssued(this, "Dimension", e.Message + $", line { lnk.Line}");
            }

            if (MarkdigTreeHelper.IsOnlyBlockElement(lnk))
            {
                var align = Attributes.ContainsKey("align") ? Attributes["align"] : "";

                switch (align)
                {
                    case "right": OutputParagraph.Format.Alignment = ParagraphAlignment.Right; break;
                    case "center": OutputParagraph.Format.Alignment = ParagraphAlignment.Center; break;
                    case "left": OutputParagraph.Format.Alignment = ParagraphAlignment.Left; break;
                }
            }

            if (MarkdigTreeHelper.IsOnlyTopParagraphElement(lnk) && Attributes.ContainsKey("watermark"))
            {
                if (img.Height.IsEmpty)
                {
                    try
                    {
                        using (System.Drawing.Image bitmapImage = System.Drawing.Image.FromFile(path))
                        {
                            if (!img.Width.IsEmpty)
                            {
                                var aspect = (double)bitmapImage.Width / bitmapImage.HorizontalResolution / ((double)bitmapImage.Height / bitmapImage.VerticalResolution);

                                OutputParagraph.Format.SpaceAfter -= img.Width / aspect;
                            }
                            else
                            {
                                OutputParagraph.Format.SpaceAfter -= Unit.FromInch((double)bitmapImage.Height / bitmapImage.VerticalResolution);
                            }
                        }
                    }
                    catch (OutOfMemoryException) { Parent.Owner.OnWarningIssued(this, "Image", "Read error: " + lnk.Url); }
                    catch (FileNotFoundException) { Parent.Owner.OnWarningIssued(this, "Image", "FileNotFound: " + lnk.Url); }
                }
                else
                {
                    OutputParagraph.Format.SpaceAfter -= img.Height;
                }
            }
        }

        private void AddTextSpan(string text, FontStyle formatting)
        {
            var f = formatting.MergeWithFont(OutputParagraph.Font, Parent.FontSize, Parent.Width, alreadyScaled: true);
            var t = Parent.Owner.OnConvertingLiteral(this, text);
            OutputParagraph.AddFormattedText(t, f);
        }

        private bool AddHtmlInline()
        {
            var hi = Inline as HtmlInline;

            if (Regex.IsMatch(hi.Tag, @"<br\s*(\/)>", RegexOptions.IgnoreCase))
            {
                OutputParagraph.AddLineBreak();
                return true;
            }

            var anchorPattern = "<a\\s+(name|id)\\s*=\"(.*)\"\\s*(\\/)?>";
            var anchor = Regex.Match(hi.Tag, anchorPattern, RegexOptions.IgnoreCase);
            if (anchor.Success)
            {
                OutputParagraph.AddBookmark(anchor.Groups[2].Value);
                return true;
            }

            Parent.Owner.OnWarningIssued(this, "Inline", "Unsupported HTML inline " + hi.Tag);
            return false;
        }

        private bool AddMath()
        {
            var inline = Inline as MathInline;
            ElementDescriptor.Attributes.Info = "math";

            var res = Parent.Owner.PluginManager.GetImage(inline.Content.ToString(), this);
            if (res?.Success ?? false)
            {
                OutputParagraph.AddImage(res.FileName);
                return true;
            }
            return false;
        }

        private void AddPlugin()
        {
            var inline = Inline as LinkInline;
            ElementDescriptor.Attributes.Info = "plugin";
            ElementDescriptor.Type = ElementType.InlinepPlugin;

            var res = Parent.Owner.PluginManager.GetImage(inline.Url.Substring("md:plugin".Length), this);
            if (res?.Success ?? false)
            {
                OutputParagraph.AddImage(res.FileName);
            }
        }

        protected CascadingStyle GetStyle()
        {
            var style = Parent.Owner.StyleManager.GetStyle(Descriptor);
            return style;
        }

        public List<SingleElementDescriptor> GetDescriptors(List<SingleElementDescriptor> descriptors = null)
        {
            descriptors = descriptors ?? new List<SingleElementDescriptor>();

            descriptors.Add(ElementDescriptor);
            if (ParentInlineConverter != null)
            {
                ParentInlineConverter.GetDescriptors(descriptors);
            }
            else if (Parent != null && Parent.GetType() != typeof(RootBlockConvertor))
            {
                (Parent as BlockConverterBase).GetDescriptors(descriptors);
            }
            return descriptors;
        }

        internal string GetInlinePlainText() => GetInlinePlainText(Inline);

        internal static string GetInlinePlainText(Inline i)
        {
            var res = new StringBuilder();

            if (i is LiteralInline)
            {
                res.Append((i as LiteralInline).Content);
            }
            if (i is CodeInline)
            {
                res.Append((i as CodeInline).Content);
            }
            if (i is ContainerInline)
            {
                foreach (var ii in i as ContainerInline)
                {
                    res.Append(GetInlinePlainText(ii));
                }
            }
            return res.ToString();
        }

        #region Special fields

        private void ExecuteInlineField(Inline inline)
        {
            var cmd = ParseSpecialField(inline);
            if (cmd == null || !cmd.Any())
            {
                Parent.Owner.OnWarningIssued(this, "Inline", $"Unknown command, line {inline.Line}");
                return;
            }

            switch (cmd[0].Key.ToLower())
            {
                case "page":
                    {
                        OutputParagraph.AddPageField();
                        break;
                    }
                case "pages":
                    {
                        OutputParagraph.AddNumPagesField();
                        break;
                    }
                case "#":
                    {
                        OutputParagraph.AddBookmark(cmd[0].Value);
                        break;
                    }

                case "sectionname":
                    {
                        OutputParagraph.AddText(Parent.Owner.MigraDocument.LastSection.Tag?.ToString() ?? "");
                        break;
                    }

                case "setsectionname":
                case "sectionbreak":
                case "pagebreak":
                    {
                        Parent.Owner.OnWarningIssued(this, "Inline", $"Command '{cmd[0].Key}' must be the only content of paragraph!, line {inline.Line}");
                        break;
                    }
                default:
                    {
                        Parent.Owner.OnWarningIssued(this, "Inline", $"Unknown command '{cmd[0].Key}', line {inline.Line}");
                        break;
                    }
            }
        }

        public static bool IsSpecialField(Inline inline)
        {
            return inline is LinkInline && (inline as LinkInline).Url.StartsWith("md:") && (inline as LinkInline).Url.Length > 3;
        }

        public static List<KeyValuePair<string, string>> ParseSpecialField(Inline inline)
        {
            if (!IsSpecialField(inline)) return new List<KeyValuePair<string, string>>();

            var cmd = (inline as LinkInline).Url.Substring(3).Trim();
            if (cmd.StartsWith("#"))
            {
                cmd = "#:" + cmd.Substring(1);
            }

            return cmd.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x =>
                {
                    var split = x.Split(new[] { ':' });
                    return new KeyValuePair<string, string>(split[0].Trim(), split.Length > 1 ? split[1].Trim() : "");
                }).ToList();
        }

        #endregion Special fields

        #region Helpers

        protected string GetTextAfter()
        {
            // Only links, images and inline code can have spacial attributes
            if (!(Inline is LinkInline || Inline is CodeInline)) return "";
            var inline = Inline;

            if (inline.Span.End < 1)
            {
                Parent.Owner.OnWarningIssued(this, "Inline", "Span with negative end value or invalid length, cannot get TextAfter, line: " + inline.Line);
                return "";
            }
            if (Inline.NextSibling != null)
            {
                if (inline.NextSibling.Span.End < 1)
                {
                    Parent.Owner.OnWarningIssued(this, "Inline", "Span with negative end value or invalid length, cannot get TextAfter, line: " + inline.Line);
                    return "";
                }
                var text = Parent.RawText.Substring(inline.Span.End + 1, inline.NextSibling.Span.Start - inline.Span.End);
                return text;
            }

            if (inline.Parent != null)
            {
                if (inline.Parent.Span.End < 0)
                {
                    Parent.Owner.OnWarningIssued(this, "Inline", "Span with negative end value or invalid length, cannot get TextAfter, line: " + inline.Line);
                    return "";
                }
                if (inline.Parent.ParentBlock == null || inline.Parent.ParentBlock.Span.End < 1)
                {
                    Parent.Owner.OnWarningIssued(this, "Inline", "Span with negative end value or invalid length, cannot get TextAfter, line: " + inline.Line);
                    return "";
                }
                var text = Parent.RawText.Substring(inline.Span.End + 1, inline.Parent.ParentBlock.Span.End - inline.Span.End);
                return text;
            }
            return "";
        }

        #endregion Helpers
    }
}