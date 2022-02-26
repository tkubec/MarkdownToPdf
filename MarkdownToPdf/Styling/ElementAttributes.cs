// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Orionsoft.MarkdownToPdfLib.Styling
{
    /// <summary>
    /// Markdown element attributes read from markdown text, see <see href="https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/GenericAttributesSpecs.md"/>
    /// </summary>
    public class ElementAttributes
    {
        internal Dictionary<string, string> Attributes { get; set; }

        /// <summary>
        /// Name of style to be used for the element
        /// </summary>
        public string Style { get; set; }

        /// <summary>
        /// Id to be used with the element. Can serve as a cross-references target
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// If the element has more markdown syntaxes, this fields specifies the used syntax. Can be used for advanced styling
        /// </summary>
        public string Markup { get; set; }

        /// <summary>
        /// Attribute of code blocks and custom containers following the opening of the block, can specify the code block programming language, e.g. ```java
        /// </summary>
        public string Info { get; set; }

        internal ElementAttributes(string text)
        {
            Attributes = new Dictionary<string, string>();

            if (text == null) return;
            var attr = Regex.Match(text, @"{(.+)}");
            if (!attr.Success) return;

            var split = attr.Groups[1].Value.Split(new[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);

            var fields = split.Select(x =>
               {
                   var kv = x.Split(new[] { '=' });
                   return new KeyValuePair<string, string>(kv[0], kv.Length > 1 ? kv[1] : "");
               }
            )
            .ToList();

            foreach (var field in fields)
            {
                if (field.Key == null) break;

                if (field.Key.StartsWith("."))
                {
                    Style = field.Key.Substring(1);
                }
                else if (field.Key.StartsWith("#"))
                {
                    Id = field.Key.Substring(1);
                }
                else if (!Attributes.ContainsKey(field.Key))
                {
                    Attributes.Add(field.Key, field.Value);
                }
            }
        }

        internal void Merge(ElementAttributes attributes)
        {
            if (attributes.Id.HasValue()) Id = attributes.Id;
            if (attributes.Style.HasValue()) Style = attributes.Style;
            if (attributes.Markup.HasValue()) Style = attributes.Markup;

            foreach (var f in attributes.Attributes)
            {
                if (!Attributes.ContainsKey(f.Key))
                {
                    Attributes.Add(f.Key, f.Value);
                }
            }
        }

        public string this[string key]
        {
            get => Attributes.ContainsKey(key) ? Attributes[key] : null;
        }

        public bool ContainsKey(string key)
        {
            return Attributes.ContainsKey(key);
        }
    }
}