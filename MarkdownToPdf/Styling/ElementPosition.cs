// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Orionsoft.MarkdownToPdfLib.Styling
{
    /// <summary>
    /// Part of <see cref="ElementAttributes"/> providing info about the elements position within it's parent element
    /// </summary>

    public class ElementPosition
    {
        public bool IsFirst { get; }
        public bool IsLast { get; }

        /// <summary>
        /// 0- baseed position in the parent container
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Number of elements in the parent container
        /// </summary>
        public int Count { get; }

        public ElementPosition(Block block)
        {
            IsFirst = block.IsFirst();
            IsLast = block.IsLast();
            Index = block.GetIndex();
            Count = block.Parent?.Count ?? 0;
        }

        public ElementPosition(Inline inline)
        {
            IsFirst = inline.IsFirst();
            IsLast = inline.IsLast();
            Index = inline.GetIndex();
            Count = inline.Parent == null ? 0 : inline.Parent.LastChild.GetIndex() + 1;
        }
    }
}