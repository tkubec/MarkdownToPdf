// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig.Syntax;
using Orionsoft.MarkdownToPdfLib.Styling;
using System.Collections.Generic;

namespace Orionsoft.MarkdownToPdfLib.Converters
{
    /// <summary>
    ///  Common interface for all types of BlockConverters
    /// </summary>
    public interface IBlockConverter : IElementConverter
    {
        /// <summary>
        /// MarkdownToPdf instance owning this object
        /// </summary>
        MarkdownToPdf Owner { get; }

        /// <summary>
        /// MigrDoc output - typically section, header, footer or table cell
        /// </summary>
        MigraDocBlockContainer OutputContainer { get; }

        /// <summary>
        /// Current Markdig block
        /// </summary>
        Block Block { get; }

        /// <summary>
        /// Style assigned by StyleManager, combined with all style ancestors, inherited styling and attributes
        /// </summary>
        CascadingStyle EvaluatedStyle { get; }

        /// <summary>
        /// Unparsed markdown source
        /// </summary>
        string RawText { get; }

        /// <summary>
        /// Unparsed markdown source split to lines
        /// </summary>
        List<string> Lines { get; }

        /// <summary>
        /// Calculated width of this block in points
        /// </summary>
        double Width { get; }

        /// <summary>
        /// Calculated fontsize of this block in points
        /// </summary>
        double FontSize { get; }
    }
}