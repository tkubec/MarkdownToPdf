// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using System.Collections.Generic;
using System.Drawing;

namespace Orionsoft.MarkdownToPdfLib.Plugins
{
    /// <summary>
    /// Result returned by a plugin
    /// </summary>
    public class HighlightingPluginResult
    {
        /// <summary>
        /// should be set only if the plugin action was successfull
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Error message in case of error. If the plugin just does not support the passed data, it must be left empty.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///  Background color of the entire block
        /// </summary>
        public Color Background { get; set; }

        /// <summary>
        /// Spans of text with formatting
        /// </summary>
        public List<HighlightedSpan> Spans { get; set; }
    }
}