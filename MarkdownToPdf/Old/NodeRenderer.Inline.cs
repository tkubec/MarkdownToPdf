using Markdig.Extensions.Tables;
using Markdig.Extensions.GenericAttributes;
using Markdig.Extensions;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static Markdig.Syntax.CodeBlock;
using Markdig.Parsers;
using System.Text.RegularExpressions;
using MigraDoc.DocumentObjectModel.Shapes;
using System.Globalization;

namespace MarkdownToPdf
{
    public partial class NodeRenderer
    {
        #region Inlines

        private void AddInline(MigrDocInlineContainer par, Inline i, InlineFormatting formatting)
        {
            if (i is LiteralInline)
            {
                AddTextSpan(par, (i as LiteralInline).Content.ToString(), formatting);
            }
            else
            if (i is EmphasisInline)
            {
                var emph = i as EmphasisInline;
                if ((emph.DelimiterChar == '*' || emph.DelimiterChar == '_') && emph.DelimiterCount == 2) formatting.Bold = true;
                if ((emph.DelimiterChar == '*' || emph.DelimiterChar == '_') && emph.DelimiterCount == 1) formatting.Italic = true;

                foreach (var ii in emph)
                {
                    AddInline(par, ii, formatting);
                }
            }
            else
            if (i is CodeInline)
            {
                var code = i as CodeInline;
                var f = par.Font.Clone();

                if (formatting.Bold) f.Bold = true;
                if (formatting.Italic) f.Italic = true;
                f.Name = par.Document.Styles[MarkdownStyleNames.InlineCode].Font.Name;
                f.Color = par.Document.Styles[MarkdownStyleNames.InlineCode].Font.Color;

                par.AddFormattedText(code.Content.ToString(), f);
            }
            else
            if (i is LineBreakInline)
            {
               // if ((i as LineBreakInline).IsHard) 
                    par.AddLineBreak();
            }
            else
            if (i is LinkInline)
            {
                var lnk = i as LinkInline;
                if (lnk.IsImage)
                {
                    // image
                    AddInlineImage(par, lnk);

                }
                else
                {
                    //  link
                    AddInlineLink(par, formatting, lnk);
                }
            }
            else
            if (i is HtmlEntityInline)
            {
                var he = i as HtmlEntityInline;
                AddTextSpan(par, he.Transcoded.ToString(), formatting);
            }
            else
            {
                //par.Paragraph.AddText(">>");
                //var footnote = par.Paragraph.AddFootnote();
                //footnote.Reference = "[1]";
                //footnote.AddParagraph("weeee");
                //footnote.Style = "Normal";
                //footnote.
                Console.WriteLine("ignored inline " + i.GetType().ToString());

                //Footnote footnote = new Footnote();
                //footnote.Reference = "[2]";
                //Paragraph par2 = footnote.Elements.AddParagraph();
                //par2.AddText("www");
                //par.Paragraph.Add(footnote);
            }
        }

        private void AddInlineLink(MigrDocInlineContainer par, InlineFormatting formatting, LinkInline lnk)
        {
            if (lnk.Url != null)
            {
                if (lnk.Url.StartsWith("md:"))
                {
                    ExecuteInlineCommand(lnk, par, false);
                    return;
                }
                var text = MarkdigTreeHelper.GetTextAfterLink(lnk, rawText);
                var attr = new SpecialAttributes(text);
                var toc = attr.Attributes.FirstOrDefault(x => Regex.Match(x.Key, "^.Toc[1-6]").Success);
                if (toc.Key != null)
                {
                    par.Style = toc.Key.Substring(1);
                    var target = lnk.Url.TrimStart(new[] { '#' });
                    var hyperlink = par.AddHyperlink(target, HyperlinkType.Local);
                    foreach (var item in lnk)
                    {
                        AddInline(hyperlink, item, formatting);
                    }
                    var fnt = par.Font.Clone();
                    if (formatting.Bold) fnt.Bold = true;
                    if (formatting.Italic) fnt.Italic = true;
                    par.Font = fnt;
                    hyperlink.AddFormattedText("\t", fnt);
                    hyperlink.AddPageRefField(target);
                    return;
                }
                var hyper = par.AddHyperlink(lnk.Url.TrimStart(new[] { '#' }), lnk.Url.StartsWith("#") ? HyperlinkType.Local : HyperlinkType.Url);
                foreach (var item in lnk)
                {
                    // hyper.Document.Styles[MarkdownStyleNames.Hyperlink].Font.Color, underline //todo
                    AddInline(hyper, item, formatting);
                }
            }
        }

        private void AddInlineImage(MigrDocInlineContainer par, LinkInline lnk)
        {
            var img = par.AddImage(lnk.Url);
            var attr = new SpecialAttributes(MarkdigTreeHelper.GetTextAfterLink(lnk, rawText));
            var width = attr.GetFirstValue("width");
            var height = attr.GetFirstValue("height");
            img.Height = MigraDocExtensions.ParseUnitEx(height, par);
            img.Width = MigraDocExtensions.ParseUnitEx(width, par);

            if (MarkdigTreeHelper.IsOnlyBlockElement(lnk))
            {
                var align = attr.GetFirstValue("align");

                switch (align)
                {
                    case "right": par.Format.Alignment = ParagraphAlignment.Right; break;
                    case "center": par.Format.Alignment = ParagraphAlignment.Center; break;
                    case "left": par.Format.Alignment = ParagraphAlignment.Left; break;
                }
            }

            if (MarkdigTreeHelper.IsOnlyTopParagraphElement(lnk))
            {
                var watermark = attr.GetFirst("watermark");
                if (watermark.Key != null)
                {
                    if (img.Height.IsEmpty)
                    {
                        try
                        {
                            var path = img.GetFilePath(null);
                            System.Drawing.Image img2 = System.Drawing.Image.FromFile(path);

                            if (!img.Width.IsEmpty)
                            {
                                var aspect = (double)img2.Width / img2.HorizontalResolution / ((double)img2.Height / img2.VerticalResolution);

                                par.Format.SpaceAfter -= img.Width / aspect;
                            }
                            else
                            {
                                par.Format.SpaceAfter -= Unit.FromInch((double)img2.Height / img2.VerticalResolution);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Img not read " + e.Message);
                        }
                    }
                    else
                    {
                        par.Format.SpaceAfter -= img.Height;
                    }
                }
            }
        }


        private static void AddTextSpan(MigrDocInlineContainer par, string text, InlineFormatting formatting)
        {
            var f = par.Font.Clone();

            if (formatting.Bold) f.Bold = true;
            if (formatting.Italic) f.Italic = true;


            par.AddFormattedText(text, f);
        }

        #endregion
    }
}
