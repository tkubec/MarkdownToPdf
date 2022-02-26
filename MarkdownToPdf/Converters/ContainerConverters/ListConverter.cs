// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig.Syntax;
using Orionsoft.MarkdownToPdfLib.Styling;

namespace Orionsoft.MarkdownToPdfLib.Converters
{
    internal class ListConverter : ContainerBlockConverter
    {
        internal ListConverter(ListBlock block, ContainerBlockConverter parent)
            : base(block, parent)
        {
            ElementDescriptor = new SingleElementDescriptor
            {
                Attributes = Attributes,
                Type = block.OrderedStart == null ? ElementType.UnorderedList : ElementType.OrderedList
            };
            Attributes.Markup = block.OrderedStart.HasValue() ? "Number" : block.BulletType.ToString();
        }
    }
}