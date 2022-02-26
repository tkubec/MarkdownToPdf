using Orionsoft.MarkdownToPdfLib.Converters;
using Orionsoft.MarkdownToPdfLib.Plugins;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;

namespace DemoHighlighter
{
    public class Highlighter : IHighlightingPlugin
    {
        public HighlightingPluginResult Convert(List<string> lines, IElementConverter converter)
        {
            if (converter is IBlockConverter && converter.Attributes.Info?.ToLower() != "python") return new HighlightingPluginResult();
            if (converter is IInlineConverter && converter.Attributes.Style?.ToLower() != "python") return new HighlightingPluginResult();

            var patterns = new Dictionary<string, Regex>  {
                { "comment", new Regex("(?<=^|[^\\\\])#.*") },
                { "string",  new Regex("(?:[rub]|br|rb)?(\"|')(?:\\\\.|(?!\\1)[^\\\\\\r\\n])*\\1", RegexOptions.IgnoreCase) },
                { "keyword", new Regex("\\b(?:_(?=\\s*:)|and|as|assert|async|await|break|case|class|continue|def|del|elif|else|except|exec|finally|for|from|global|if|import|in|is|lambda|match|nonlocal|not|or|pass|print|raise|return|try|while|with|yield)\\b") },
                { "builtin", new Regex("\\b(?:__import__|abs|all|any|apply|ascii|basestring|bin|bool|buffer|bytearray|bytes|callable|chr|classmethod|cmp|coerce|compile|complex|delattr|dict|dir|divmod|enumerate|eval|execfile|file|filter|float|format|frozenset|getattr|globals|hasattr|hash|help|hex|id|input|int|intern|isinstance|issubclass|iter|len|list|locals|long|map|max|memoryview|min|next|object|oct|open|ord|pow|property|range|raw_input|reduce|reload|repr|reversed|round|set|setattr|slice|sorted|staticmethod|str|sum|super|tuple|type|unichr|unicode|vars|xrange|zip)\\b") },
                { "boolean", new Regex("\\b(?:False|None|True)\\b") },
                { "number", new Regex("\\b0(?:b(?:_?[01])+|o(?:_?[0-7])+|x(?:_?[a-f0-9])+)\\b|(?:\\b\\d+(?:_\\d+)*(?:\\.(?:\\d+(?:_\\d+)*)?)?|\\B\\.\\d+(?:_\\d+)*)(?:e[+-]?\\d+(?:_\\d+)*)?j?(?!\\w)", RegexOptions.IgnoreCase)},
                { "operator", new Regex("[-+%=]=?|!=|:=|\\*\\*?=?|\\/\\/?=?|<[<=>]?|>[=>]?|[&|^~]") },
                { "punct", new Regex("[{}[\\];(),.:]") }
        };

            var theme = new Dictionary<string, Color>  {
                { "comment", Color.Gray },
                { "string",  Color.FromArgb(0, 40,100,10)  },
                { "keyword", Color.DarkBlue },
                { "builtin", Color.DarkMagenta },
                { "boolean", Color.Pink },
                { "number",  Color.DarkCyan },
                { "operator",Color.Coral },
                { "punct", Color.DarkCyan }
            };

            var spans = new List<HighlightedSpan>();

            foreach (var l in lines)
            {
                var pos = 0;
                while (pos < l.Length)
                {
                    Match bestMatch = null;
                    var bestOffset = l.Length + 1;
                    var bestId = "";

                    foreach (var p in patterns)
                    {
                        var res = p.Value.Match(l, pos);
                        if (!res.Success) continue;
                        if (res.Groups[0].Index < bestOffset)
                        {
                            bestOffset = res.Captures[0].Index;
                            bestMatch = res;
                            bestId = p.Key;
                        }
                    }
                    if (bestMatch != null)
                    {
                        if (pos < bestOffset)
                        {
                            spans.Add(new HighlightedSpan { Text = l.Substring(pos, bestOffset - pos) });
                        }
                        spans.Add(new HighlightedSpan
                        {
                            Text = l.Substring(bestOffset, bestMatch.Captures[0].Length),
                            Color = theme[bestId]
                        });
                        pos = bestOffset + bestMatch.Captures[0].Length;
                    }
                    else
                    {
                        spans.Add(new HighlightedSpan { Text = l.Substring(pos, l.Length - pos) });
                        break;
                    }
                }
            }

            return new HighlightingPluginResult { Spans = spans, Success = true, Background = Color.FromArgb(255, 0xf5, 0xf2, 0xf0) };
        }
    }
}