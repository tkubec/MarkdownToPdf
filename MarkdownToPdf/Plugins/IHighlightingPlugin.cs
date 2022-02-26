// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Orionsoft.MarkdownToPdfLib.Converters;
using System.Collections.Generic;

namespace Orionsoft.MarkdownToPdfLib.Plugins
{
    // Any highlighting plugin must use this inteface
    public interface IHighlightingPlugin
    {
        /// <summary>
        /// Main exposed function that should perform the conversion
        /// </summary>
        /// <param name="lines">lines of code to highlight</param>
        /// <param name="converter">the converter requesting the higlighting</param>
        /// <returns></returns>
        HighlightingPluginResult Convert(List<string> lines, IElementConverter converter);
    }
}