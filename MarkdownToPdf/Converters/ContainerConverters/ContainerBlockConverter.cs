// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig.Extensions.CustomContainers;
using Markdig.Extensions.Footnotes;
using Markdig.Extensions.Tables;
using Markdig.Syntax;
using Orionsoft.MarkdownToPdfLib.Styling;
using System.Collections.Generic;
using System.Linq;

namespace Orionsoft.MarkdownToPdfLib.Converters
{
    internal abstract class ContainerBlockConverter : BlockConverterBase
    {
        protected ContainerBlockConverter(ContainerBlock block, ContainerBlockConverter parent)
            : base(block, parent)
        {
            Attributes = new ElementAttributes(GetTextBefore());
        }

        protected override bool CreateOutput()
        {
            return true;
        }

        protected override void PrepareStyling()
        {
            EvaluatedStyle = GetStyle().Eval();
            PrepareVerticalInheritedMargins();

            if (Parent != null)
            {
                EvaluatedStyle.Font = EvaluatedStyle.Font.MergeWith(Parent.EvaluatedStyle.Font);
                EvaluatedStyle.Background = EvaluatedStyle.Background.IsEmpty ? Parent.EvaluatedStyle.Background : EvaluatedStyle.Background;

                FontSize = EvaluatedStyle.Font.Size.Eval(Parent.FontSize, Parent.Width);
                Width = Parent.Width - (EvaluatedStyle.Margin.Left + EvaluatedStyle.Margin.Right + EvaluatedStyle.Padding.Left + EvaluatedStyle.Padding.Right)
                    .Eval(FontSize, Parent.Width);
            }
        }

        protected override void ApplyStyling()
        {
        }

        protected override void ConvertContent() => ConvertBlocks(Block as ContainerBlock);

        protected virtual void ConvertBlocks(IEnumerable<Block> blocks)
        {
            foreach (var block in blocks)
            {
                ConvertBlock(block);
            }
        }

        protected virtual bool ConvertBlock(Block block)
        {
            var type = block.GetType();

            if (type == typeof(ParagraphBlock))
            {
                var conv = new ParagraphBlockConverter(block as ParagraphBlock, this);
                conv.Convert();
                return true;
            }
            if (type == typeof(HeadingBlock))
            {
                var conv = new HeadinghBlockConverter(block as HeadingBlock, this);
                conv.Convert();
                return true;
            }

            if (type == typeof(ListBlock))
            {
                var conv = new ListConverter(block as ListBlock, this);
                conv.Convert();
                return true;
            }

            if (type == typeof(ListItemBlock))
            {
                var conv = new ListItemConverter(block as ListItemBlock, this);
                conv.Convert();
                return true;
            }

            if (type == typeof(QuoteBlock))
            {
                var conv = new QuoteBlockConverter(block as QuoteBlock, this);
                conv.Convert();
                return true;
            }

            if (type == typeof(ThematicBreakBlock))
            {
                var conv = new ThematicBreakBlockConverter(block as ThematicBreakBlock, this);
                conv.Convert();
                return true;
            }

            if (type == typeof(CodeBlock) || type == typeof(FencedCodeBlock))
            {
                var conv = new CodeBlockConverter(block as CodeBlock, this);
                conv.Convert();
                return true;
            }

            if (type == typeof(Table))
            {
                var conv = new TableBlockConverter(block as Table, this);
                conv.Convert();
                return true;
            }

            if (type == typeof(FootnoteGroup))
            {
                var conv = new FootnoteGroupConverter(block as FootnoteGroup, this);
                conv.Convert();
                return true;
            }

            if (type == typeof(CustomContainer))
            {
                var conv = new CustomContainerConverter(block as CustomContainer, this);
                conv.Convert();
                return true;
            }

            if (type == typeof(LinkReferenceDefinitionGroup))
            {
                // Link references are accessed directly from FootNote converters
                return true;
            }

            Owner.OnWarningIssued(this, "Block", $"Unsupported: {block.GetType().Name}, line {block.Line}");
            return false;
        }

        #region Attribute helpers

        protected string GetTextBefore()
        {
            var block = Block as ContainerBlock;
            if (!block.Any() || RawText == null) return "";

            if (block is TableRow || block is TableCell                         // cannot have attributtes before
                 || block is FootnoteGroup                                      // cannot have attributtes before
                 || block is Footnote                                           // has own GetTextBefore
               ) return "";

            if (block is Table && block.Parser is GridTableParser) return "";    // has own GetTextBefore

            if (block.First().Span.Start < 0 || block.Span.Start < 0 || block.First().Span.Start < block.Span.Start || block.Span.Start == 0 && block.Line > 0)
            {
                Owner.OnWarningIssued(this, "ContainerBlock", "Span with negative end value or invalid length, cannot get TextBefore, line: " + block.Line);
                return "";
            }

            var prev = GetPrevSibling(Block);
            var start = 0;

            if (prev != null)
            {
                start = prev.Span.End + 1;
            }
            else
            {
                if (Block.Parent == null) return null;
                start = Block.Parent.Span.Start;

                // fix for span starts set to 0
                if (start == 0 && Block.Parent.Line > 0)
                {
                    start = Lines.Take(Block.Parent.Line).Sum(x => x.Length);
                }
            }

            var end = Block.Span.Start;

            // fix for span starts set to 0
            if (end == 0 && Block.Line > 0)
            {
                end = Lines.Take(Block.Line + 1).Sum(x => x.Length);
            }

            var t = RawText.Substring(start, end - start);
            return t;
        }

        private Block GetPrevSibling(Block block)
        {
            if (block.Parent == null) return null;
            var currentBlockIndex = block.Parent.IndexOf(block);
            var prevSibling = currentBlockIndex == 0 ? null : block.Parent[currentBlockIndex - 1];
            return prevSibling;
        }

        #endregion Attribute helpers
    }
}