// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using System;
using System.Collections.Generic;

namespace Orionsoft.MarkdownToPdfLib.Styling
{
    /// <summary>
    /// Fluent binding of styles
    /// </summary>
    public class SelectorBuilder
    {
        private readonly StyleManager styleManager;

        private readonly List<StyleSelector> selectors;

        internal SelectorBuilder(StyleManager styleManager, ElementType type, string styleName)
        {
            this.styleManager = styleManager;
            selectors = new List<StyleSelector>
            {
                new StyleSelector { ElementType = type, StyleName = styleName }
            };
        }

        /// <summary>
        /// Declares that the style is used only if previous selector in chain has parent of specified type (and name)
        /// </summary>
        public SelectorBuilder WithParent(ElementType type, string styleName = "")
        {
            selectors.Add(new StyleSelector { ElementType = type, StyleName = styleName, SelectorType = StyleSelector.SelectorTypes.Parent });
            return this;
        }

        /// <summary>
        /// Declares that the style is used only if previous selector in chain has ancestor of specified type (and name)
        /// </summary>
        public SelectorBuilder WithAncestor(ElementType type, string styleName = "")
        {
            selectors.Add(new StyleSelector { ElementType = type, StyleName = styleName, SelectorType = StyleSelector.SelectorTypes.Ancestor });
            return this;
        }

        /// <summary>
        /// Declares that the style is used only if the filter condition is true. Usefull for advanced styling.
        /// </summary>
        /// <param name="filter">function deciding whether the style matches according to the passed StylingDescriptor</param>
        /// <returns></returns>
        public SelectorBuilder Where(Func<StylingDescriptor, bool> filter)
        {
            selectors.Add(new StyleSelector { SelectorType = StyleSelector.SelectorTypes.Filter, Filter = filter });
            return this;
        }

        /// <summary>
        /// Binds the style to markdown elements according to conditions based on preceeding selector chain.
        /// </summary>
        public void Bind(CascadingStyle style)
        {
            styleManager.Bind(selectors, (style, null));
        }

        /// <inheritdoc cref="Bind" />
        public void Bind(string styleName)
        {
            Bind(styleManager.Styles[styleName]);
        }

        /// <summary>
        /// Binds the style to markdown elements according to conditions based on preceeding selector chain. When the style is evaluated, the modification method is called and can adjust the evaluated style
        /// </summary>
        public void BindAndModify(CascadingStyle style, Action<CascadingStyle, StylingDescriptor> modificationMethod)
        {
            styleManager.Bind(selectors, (style, modificationMethod));
        }

        /// <inheritdoc cref="BindAndModify" />
        public void BindAndModify(string styleName, Action<CascadingStyle, StylingDescriptor> modificationMethod)
        {
            BindAndModify(styleManager.Styles[styleName], modificationMethod);
        }
    }
}