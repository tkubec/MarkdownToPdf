// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Orionsoft.MarkdownToPdfLib.Styling;

namespace Orionsoft.MarkdownToPdfLib.Converters
{
    /// <summary>
    /// Common interface for  <see cref="IInlineConverter" />s and <see cref="IBlockConverter"/>s
    /// </summary>
    public interface IElementConverter
    {
        /// <summary>
        /// Block converter owning this element
        /// </summary>
        IBlockConverter Parent { get; }

        /// <summary>
        /// Special attributes bound to this element that were read from markdown text (enclosed in curly braces), e.g. style name
        /// </summary>
        ElementAttributes Attributes { get; }

        /// <summary>
        /// Hierarchy of element descriptors of this block and all it's ancestors; used by StyleManager
        /// </summary>
        StylingDescriptor Descriptor { get; }
    }
}