// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig.Syntax.Inlines;

namespace Orionsoft.MarkdownToPdfLib.Converters
{
    /// <summary>
    ///  Common interface for all types of InlineConverters
    /// </summary>

    public interface IInlineConverter : IElementConverter
    {
        /// <summary>
        /// Current Markdig inline element
        /// </summary>
        Inline Inline { get; }

        /// <summary>
        /// MigraDoc paragraph (or hyperlink content) that is currently being created
        /// </summary>
        MigrDocInlineContainer OutputParagraph { get; }

        /// <summary>
        /// InlineConverter that owns this inline - applies to nested Inlines. BlockConverter owning this inline and all ancestor inlines is <see cref="IElementConverter.Parent"/>
        /// </summary>
        IInlineConverter ParentInlineConverter { get; }
    }
}