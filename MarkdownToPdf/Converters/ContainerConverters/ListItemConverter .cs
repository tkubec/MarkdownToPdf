// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig.Syntax;
using Orionsoft.MarkdownToPdfLib.Styling;

namespace Orionsoft.MarkdownToPdfLib.Converters
{
    internal class ListItemConverter : ContainerBlockConverter
    {
        internal ListItemConverter(ListItemBlock block, ContainerBlockConverter parent)
            : base(block, parent)
        {
            ElementDescriptor = new SingleElementDescriptor
            {
                Attributes = Attributes,
                Type = Parent.ElementDescriptor.Type == ElementType.OrderedList ? ElementType.OrderedListItem : ElementType.UnorderedListItem,
                Position = new ElementPosition(Block)
            };
        }
    }
}