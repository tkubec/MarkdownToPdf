// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig.Extensions.CustomContainers;
using Orionsoft.MarkdownToPdfLib.Styling;

namespace Orionsoft.MarkdownToPdfLib.Converters
{
    internal class CustomContainerConverter : ContainerBlockConverter
    {
        internal CustomContainerConverter(CustomContainer block, ContainerBlockConverter parent)
            : base(block, parent)
        {
            Attributes.Info = block.Info;
            ElementDescriptor = new SingleElementDescriptor { Attributes = Attributes, Type = ElementType.CustomContainer, Position = new ElementPosition(Block) };
        }
    }
}