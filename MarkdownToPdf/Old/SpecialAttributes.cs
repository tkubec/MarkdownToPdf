using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Markdig.Extensions.Tables;
using Markdig.Extensions.GenericAttributes;
using Markdig.Extensions;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace MarkdownToPdf
{
    public class SpecialAttributes
    {
        private string text;
        public List<KeyValuePair<string, string>> Attributes { get; set; }
        public SpecialAttributes(string text)
        {
            Attributes = new List<KeyValuePair<string, string>>();
            this.text = text;
            var attr = Regex.Match(text, @"\s*#*\s*{(.+)}");
            if (!attr.Success) return;
            
            var split = attr.Groups[1].Value.Split(new[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
            
            Attributes = split.Select(x =>
                {
                    var kv = x.Split(new[] { '=' });
                    return new KeyValuePair<string, string>(kv[0], kv.Length > 1 ? kv[1] : "");
                }
            )
            .ToList();
        }

        public string GetId()
        {
            return Attributes.FirstOrDefault(x => x.Key.StartsWith("#")).Key;
        }

        public string GetFirstValue(string key)
        {
            return Attributes.FirstOrDefault(x => x.Key == key).Value;
        }

        public KeyValuePair<string, string> GetFirst(string key)
        {
            return Attributes.FirstOrDefault(x => x.Key == key);
        }

        public List<string> GetStyles()
        {
            return Attributes.Where(x => x.Key.StartsWith(".")). Select(x => x.Key.Substring(1)).ToList();
        }
    }
}