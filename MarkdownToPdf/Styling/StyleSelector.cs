// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using System;
using System.Collections.Generic;

namespace Orionsoft.MarkdownToPdfLib.Styling
{
    internal class StyleSelector
    {
        public ElementType ElementType { get; set; }
        public SelectorTypes SelectorType { get; set; }
        public string StyleName { get; set; }
        public Func<StylingDescriptor, bool> Filter { get; internal set; }

        public enum SelectorTypes
        {
            Base, Parent, Ancestor, Filter
        }

        public bool IsEqual(StyleSelector other)
        {
            return ElementType == other.ElementType && SelectorType == other.SelectorType && StyleName == other.StyleName && Filter == other.Filter;
        }

        public static bool IsEqualSelectorList(List<StyleSelector> a, List<StyleSelector> b)
        {
            if (a.Count != b.Count) return false;

            for (int i = 0; i < a.Count; i++)
            {
                if (!a[i].IsEqual(b[i])) return false;
            }
            return true;
        }
    }
}