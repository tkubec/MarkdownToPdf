// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig.Syntax;
using MigraDoc.DocumentObjectModel;
using Orionsoft.MarkdownToPdfLib.Plugins;
using Orionsoft.MarkdownToPdfLib.Styling;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Orionsoft.MarkdownToPdfLib.Converters
{
    internal class CodeBlockConverter : LeafBlockConverter<CodeBlock>
    {
        private List<HighlightedSpan> highlightedSpans = new List<HighlightedSpan>();

        internal CodeBlockConverter(CodeBlock block, ContainerBlockConverter parent)
            : base(block, parent)
        {
            ElementDescriptor = new SingleElementDescriptor
            {
                Attributes = Attributes,
                Type = ElementType.Code,
                Position = new ElementPosition(Block),
                PlainText = GetPlainText(CurrentBlock)
            };
            Attributes.Info = (block as FencedCodeBlock)?.Info;
        }

        protected override void PrepareStyling()
        {
            base.PrepareStyling();

            // We need to preprocess the lines and  access thhe plugins now, bwcause they change the styling

            var linesGroup = Block is FencedCodeBlock ? (Block as FencedCodeBlock).Lines : (Block as CodeBlock).Lines;
            var lines = new List<string>();

            var cnt = 0;
            foreach (var l in linesGroup.Lines)
            {
                if (cnt >= linesGroup.Count) break;
                lines.Add(l.ToString() + "\n");
                cnt++;
            }
            var pluginResult = Owner.PluginManager.Highlight(lines, this);

            if (pluginResult == null || !pluginResult.Success)
            {
                foreach (var l in lines)
                {
                    highlightedSpans.Add(new HighlightedSpan { Text = DeTab(l) });
                }
            }
            else
            {
                highlightedSpans = pluginResult.Spans;
                if (!pluginResult.Background.IsEmpty)
                {
                    EvaluatedStyle.Background = Color.FromArgb(255, pluginResult.Background.R, pluginResult.Background.G, pluginResult.Background.B);
                }
            }
        }

        protected override void ConvertContent()
        {
            foreach (var highlighted in highlightedSpans)
            {
                var split = Regex.Split(highlighted.Text, @"(?=  )(?<=[^ ])|(?=[^ ])(?<=  )|(?=\n)|(?<=\n.+)");
                if (split == null) return;

                var f = OutputParagraph.Font.Clone();
                f.Color = Color.FromRgb(highlighted.Color.R, highlighted.Color.G, highlighted.Color.B);
                f.Bold = highlighted.Bold || f.Bold;
                f.Italic = highlighted.Italic || f.Italic;
                f.Underline = highlighted.Underline ? Underline.Single : f.Underline;

                var span = OutputParagraph.AddFormattedText("", f);
                foreach (var ch in split)
                {
                    if (ch.IndexOf("  ") > -1)
                    {
                        span.AddSpace(ch.Length);
                    }
                    else if (ch == "\n") span.AddLineBreak();
                    else span.AddText(ch);
                }
            }
        }

        protected override string GetPlainText(LeafBlock block)
        {
            if (CurrentBlock.CodeBlockLines == null) return "";
            var res = new StringBuilder();
            foreach (var i in CurrentBlock.CodeBlockLines)
            {
                res.Append(i.ToString());
            }
            return res.ToString();
        }

        private string DeTab(string l, int tabWidth = 4)
        {
            var pos = 0;
            var sb = new StringBuilder();
            foreach (var ch in l)
            {
                if (ch != '\t')
                {
                    sb.Append(ch);
                    pos++;
                }
                else
                {
                    var spaces = (pos + 1) % tabWidth;
                    sb.Append(' ', spaces);
                    pos += spaces;
                }
            }
            return sb.ToString();
        }
    }
}