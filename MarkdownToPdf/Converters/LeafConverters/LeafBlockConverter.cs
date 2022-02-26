// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig.Extensions.Footnotes;
using Markdig.Extensions.TaskLists;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using MigraDoc.DocumentObjectModel;
using Orionsoft.MarkdownToPdfLib.Styling;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Orionsoft.MarkdownToPdfLib.Converters
{
    internal abstract class LeafBlockConverter<TLeafBlock> : BlockConverterBase
        where TLeafBlock : LeafBlock
    {
        public MigrDocInlineContainer OutputParagraph { get; protected set; }
        public TLeafBlock CurrentBlock { get; }

        protected LeafBlockConverter(LeafBlock block, ContainerBlockConverter parent)
            : base(block, parent)
        {
            CurrentBlock = block as TLeafBlock;
            Attributes = new ElementAttributes(GetTextBefore());
            Attributes.Merge(new ElementAttributes(GetTextAfter()));
        }

        protected override void PrepareStyling()
        {
            EvaluatedStyle = GetStyle().Eval();
            PrepareVerticalInheritedMargins();

            EvaluatedStyle.Font = EvaluatedStyle.Font.MergeWith(Parent.EvaluatedStyle.Font);
            EvaluatedStyle.Background = EvaluatedStyle.Background.IsEmpty ? Parent.EvaluatedStyle.Background : EvaluatedStyle.Background;
            EvaluatedStyle.Paragraph.Alignment = EvaluatedStyle.Paragraph.Alignment.HasValue ? EvaluatedStyle.Paragraph.Alignment : Parent.EvaluatedStyle.Paragraph.Alignment;

            if (Attributes.ContainsKey("align"))
            {
                var align = Attributes["align"];

                switch (align)
                {
                    case "justify": EvaluatedStyle.Paragraph.Alignment = ParagraphAlignment.Justify; break;
                    case "left": EvaluatedStyle.Paragraph.Alignment = ParagraphAlignment.Left; break;
                    case "right": EvaluatedStyle.Paragraph.Alignment = ParagraphAlignment.Right; break;
                    case "center": EvaluatedStyle.Paragraph.Alignment = ParagraphAlignment.Center; break;
                }
            }

            FontSize = EvaluatedStyle.Font.Size.Eval(Parent.FontSize, Parent.Width);
            Width = Parent.Width - (EvaluatedStyle.Margin.Left + EvaluatedStyle.Margin.Right + EvaluatedStyle.Padding.Left + EvaluatedStyle.Padding.Right).Eval(FontSize, Parent.Width);
        }

        protected override void AdjustInheritedHorizontalMargins()
        {
            topMarginsPending = ApplyHorizontalInheritedMargin(BoxSide.Top);
            bottomMarginsPending = ApplyHorizontalInheritedMargin(BoxSide.Bottom);
            EvaluatedStyle.Padding.Top = EvaluatedStyle.Padding.Top.IsEmpty ? 0 : EvaluatedStyle.Padding.Top;
            EvaluatedStyle.Padding.Bottom = EvaluatedStyle.Padding.Top.IsEmpty ? 0 : EvaluatedStyle.Padding.Bottom;
            EvaluatedStyle.Margin.Top = EvaluatedStyle.Margin.Top.IsEmpty ? 0 : EvaluatedStyle.Margin.Top;
            EvaluatedStyle.Margin.Bottom = EvaluatedStyle.Margin.Top.IsEmpty ? 0 : EvaluatedStyle.Margin.Bottom;
        }

        protected override void ApplyStyling()
        {
            if (OutputParagraph == null) return;

            OutputParagraph.Format = EvaluatedStyle.Merge(OutputParagraph.Format, FontSize, Width);
            OutputParagraph.Format.Shading.Color = EvaluatedStyle.Background;

            if (Attributes.Id.HasValue())
            {
                OutputParagraph.AddBookmark(Attributes.Id);
            }

            TransformMarginsToIndents();
        }

        private void TransformMarginsToIndents()
        {
            OutputParagraph.Format.LeftIndent =
                (EvaluatedStyle.Border.Left.Width +
                EvaluatedStyle.Margin.Left +
                EvaluatedStyle.Padding.Left).Eval(FontSize, Width);

            OutputParagraph.Format.RightIndent =
                (EvaluatedStyle.Border.Right.Width +
                EvaluatedStyle.Margin.Right +
                EvaluatedStyle.Padding.Right).Eval(FontSize, Width);

            OutputParagraph.Format.SpaceBefore =
                EvaluatedStyle.Margin.Top.Eval(FontSize, Width);

            OutputParagraph.Format.SpaceAfter =
                EvaluatedStyle.Margin.Bottom.Eval(FontSize, Width);

            OutputParagraph.Format.Borders.DistanceFromLeft = EvaluatedStyle.Padding.Left.Eval(FontSize, Width);
            OutputParagraph.Format.Borders.DistanceFromRight = EvaluatedStyle.Padding.Right.Eval(FontSize, Width);
            OutputParagraph.Format.Borders.DistanceFromTop = EvaluatedStyle.Padding.Top.Eval(FontSize, Width);
            OutputParagraph.Format.Borders.DistanceFromBottom = EvaluatedStyle.Padding.Bottom.Eval(FontSize, Width);
        }

        protected override void ConvertContent()
        {
            ConvertInlines(Block as LeafBlock);
        }

        protected override bool CreateOutput()
        {
            OutputParagraph = OutputContainer.AddParagraph();
            return true;
        }

        private void ConvertInlines(LeafBlock blocks)
        {
            foreach (var inline in blocks?.Inline ?? new ContainerInline())
            {
                // fixing additional linebreaks in case of checklists and elements with attributes
                if (inline.PreviousSibling?.GetType() == typeof(TaskList) && inline.GetType() == typeof(LineBreakInline)) continue;
                if (inline.PreviousSibling == null && inline.GetType() == typeof(LineBreakInline) && !(inline as LineBreakInline).IsHard) continue;

                ConvertInline(inline, new FontStyle());
            }
        }

        public override string ToString()
        {
            return GetPlainText(Block as LeafBlock);
        }

        private void ConvertInline(Inline inline, FontStyle formatting)
        {
            var c = new InlineConverter(inline, null, this, OutputParagraph, formatting);
            c.Convert();
        }

        #region Helpers

        protected string GetTextBefore()
        {
            var block = Block as LeafBlock;
            if (block is CodeBlock) return GetTextBeforeAlt(block);

            if (block.Inline == null || !block.Inline.Any()) return "";

            if (block.Inline.First().Span.Start < 0 || block.Span.Start < 0 || block.Inline.First().Span.Start < block.Span.Start)
            {
                Owner.OnWarningIssued(this, "LeafBlock", "Span with negative end value or invalid length, cannot get TextBefore, line: " + block.Line);
                return "";
            }

            var text = RawText.Substring(block.Span.Start, block.Inline.First().Span.Start - block.Span.Start);

            if (Regex.IsMatch(text, "{[^}]*}")) return text;

            // now, let's try an alternative method
            return GetTextBeforeAlt(block);
        }

        private string GetTextBeforeAlt(LeafBlock block)
        {
            if (block.Line < 1) return "";

            var previousLine = Lines[block.Line - 1];

            // is it the only content of the line?
            var rx = Regex.Match(previousLine, @"^\s*{[^}]+}\s$");
            if (!rx.Success) return "";

            // does it belong to this or to the parent?
            if (RawText[block.Span.Start - 1] != '\n') return ""; // the block does not start from the beginning, so it is nested

            return previousLine;
        }

        protected string GetTextAfter()
        {
            var block = Block as LeafBlock;
            if (block.Inline == null || !block.Inline.Any()) return "";

            // footnote backlinks do not have span info
            var lastInline = block.Inline.Last(x => !(x is FootnoteLink && (x as FootnoteLink).IsBackLink));

            if (lastInline.Span.End < 1 || block.Span.End < 1 || lastInline.Span.End > block.Span.End)
            {
                Owner.OnWarningIssued(this, "Block", "Span with negative end value or invalid length, cannot get TextAfter, line: " + block.Line);
                return "";
            }
            var text = RawText.Substring(lastInline.Span.End + 1, block.Span.End - lastInline.Span.End);

            // if the last inline is a link without space after it (a one character literal), the attribute belongs to it, not to this block
            if (!(lastInline is LinkInline) && Regex.IsMatch(text, "{[^}]*}"))
            {
                return text;
            }

            // now, let's try an alternative method
            var eol = RawText.IndexOf('\n', block.Span.End + 1);
            var followingText = eol >= 0 ? RawText.Substring(block.Span.End + 1, eol - block.Span.End) : RawText.Substring(block.Span.End + 1);

            // Hash sign after heading separates inline style from paragraph style, so it is easy to distinguish between inline style and block style
            var rx = Regex.Match(followingText, @"#+ *\{[^}]+\}\w*\r?\n");
            if (rx.Success)
            {
                return rx.Groups[0].Value;
            }

            rx = Regex.Match(followingText, @"\{[^}]+\}\w*\r?\n");
            if (rx.Success)
            {
                if (lastInline is LinkInline && RawText.Substring(block.Span.End, 1) != " ")
                {
                    // This attribute belongs to the link
                    return "";
                }
                return rx.Groups[0].Value;
            }

            return "";
        }

        protected virtual string GetPlainText(LeafBlock block)
        {
            if (block.Inline == null) return "";
            var res = new StringBuilder();
            foreach (var i in block.Inline)
            {
                res.Append(InlineConverter.GetInlinePlainText(i));
            }
            return res.ToString();
        }

        protected string GetInlinePlainText(Inline i)
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

        #endregion Helpers
    }
}