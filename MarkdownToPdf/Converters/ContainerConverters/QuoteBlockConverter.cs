// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig.Syntax;
using Orionsoft.MarkdownToPdfLib.Styling;

namespace Orionsoft.MarkdownToPdfLib.Converters
{
    internal class QuoteBlockConverter : ContainerBlockConverter
    {
        internal QuoteBlockConverter(QuoteBlock block, ContainerBlockConverter parent)
            : base(block, parent)
        {
            ElementDescriptor = new SingleElementDescriptor { Attributes = Attributes, Type = ElementType.Quote, Position = new ElementPosition(Block) };
        }
    }
}