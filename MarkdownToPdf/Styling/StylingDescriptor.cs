// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using System.Collections.Generic;
using System.Linq;

namespace Orionsoft.MarkdownToPdfLib.Styling
{
    /// <summary>
    /// Descriptor for markdown element and all it's ancestors, containing their type, attributes and position in markdown document tree
    /// </summary>

    public class StylingDescriptor
    {
        /// <summary>
        /// Descriptors for this element and each it's ancestor
        /// </summary>
        public readonly List<SingleElementDescriptor> Descriptors;

        /// <summary>
        /// Descriptor of current element
        /// </summary>
        public SingleElementDescriptor CurrentElement { get => Descriptors.FirstOrDefault(); }

        public SingleElementDescriptor this[int key]
        {
            get => Descriptors[key];
        }

        internal StylingDescriptor(List<SingleElementDescriptor> descriptors)
        {
            this.Descriptors = descriptors ?? new List<SingleElementDescriptor>();
        }

        public bool HasParent(ElementType t, string styleName = "")
        {
            if (Descriptors.Count < 2) return false;
            return Descriptors[1].Type == t && (Descriptors[1].Attributes.Style == styleName || !styleName.HasValue());
        }

        public bool HasAncestor(ElementType t, string styleName = "")
        {
            return Descriptors.Skip(1).Any(x => x.Type == t && (x.Attributes.Style == styleName || !styleName.HasValue()));
        }

        public bool HasParentWithId(string id)
        {
            if (Descriptors.Count < 2) return false;
            return Descriptors[1].Attributes.Id == id;
        }

        public bool HasAncestorWithId(string id)
        {
            return Descriptors.Skip(1).Any(x => x.Attributes.Id == id);
        }
    }
}