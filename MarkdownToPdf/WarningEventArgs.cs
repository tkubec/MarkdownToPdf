// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using System;

namespace Orionsoft.MarkdownToPdfLib
{
    /// <summary>
    /// Arguments of <see cref="MarkdownToPdf.WarningIssued"/> event
    /// </summary>
    public class WarningEventArgs : EventArgs
    {
        /// <summary>
        /// Warning category
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        ///  Detailed warning message
        /// </summary>
        public string Message { get; set; }
    }
}