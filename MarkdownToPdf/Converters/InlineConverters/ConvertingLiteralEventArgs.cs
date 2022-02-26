// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using System;

namespace Orionsoft.MarkdownToPdfLib.Converters
{
    /// <summary>
    /// Arguments of <see cref="MarkdownToPdf.ConvertingLiteral"/> Event invoked just before conversion of a literal text span starts.
    /// The text can be modified by the event handler and used by the converter afterwars.
    /// </summary>
    public class ConvertingLiteralEventArgs : EventArgs
    {
        public string Text { get; set; }
    }
}