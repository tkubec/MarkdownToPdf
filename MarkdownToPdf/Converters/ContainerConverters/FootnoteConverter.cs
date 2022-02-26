// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig.Extensions.Footnotes;
using Markdig.Syntax;
using Orionsoft.MarkdownToPdfLib.Styling;
using System.Linq;
using System.Text.RegularExpressions;

namespace Orionsoft.MarkdownToPdfLib.Converters
{
    internal class FootnoteConverter : ContainerBlockConverter
    {
        internal FootnoteConverter(Markdig.Extensions.Footnotes.Footnote block, ContainerBlockConverter parent)
            : base(block, parent)
        {
            Attributes = new ElementAttributes(CustomGetTextBefore());
            ElementDescriptor = new SingleElementDescriptor { Attributes = Attributes, Type = ElementType.Footnote, Position = new ElementPosition(Block) };
        }

        protected string CustomGetTextBefore()
        {
            var p = Block.Parent;
            while (p != null && p.Parent != null) p = p.Parent;
            if (p != null && p.Any(x => x is LinkReferenceDefinitionGroup))
            {
                var lrg = p.First(x => x is LinkReferenceDefinitionGroup) as LinkReferenceDefinitionGroup;

                if (lrg.FirstOrDefault(x => x is FootnoteLinkReferenceDefinition && (x as FootnoteLinkReferenceDefinition).Footnote == Block) is FootnoteLinkReferenceDefinition fng && fng.Line > 0)
                {
                    var text = Lines[fng.Line - 1].Trim();
                    if (Regex.IsMatch(text, @"^\{.+\}$")) return text;
                }
            }
            return "";
        }
    }
}