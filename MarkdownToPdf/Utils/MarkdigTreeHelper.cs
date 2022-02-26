// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System.Linq;

namespace Orionsoft.MarkdownToPdfLib
{
    /// <summary>
    ///  Helper methods for traversing markdown tree, getting special attributes, etc
    /// </summary>
    internal static class MarkdigTreeHelper
    {
        public static bool IsOnlyBlockElement(LinkInline inline)
        {
            var isSingle = (inline.Parent?.Count() ?? 0) == 1;
            if (!isSingle) return false;
            if (inline.Parent.Parent != null) return false;
            if (inline.Parent.ParentBlock == null || inline.Parent.ParentBlock.Parent == null) return false;
            return true;
        }

        public static bool IsOnlyTopParagraphElement(LinkInline inline)
        {
            if (!IsOnlyBlockElement(inline)) return false;
            if (inline.Parent.ParentBlock.GetType() != typeof(ParagraphBlock)) return false;
            if (inline.Parent.ParentBlock.Parent.GetType() == typeof(MarkdownDocument)) return true;
            return false;
        }
    }
}