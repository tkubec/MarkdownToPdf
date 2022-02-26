// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

namespace Orionsoft.MarkdownToPdfLib
{
    internal static class StringExtensions
    {
        public static bool HasValue(this string str)
        {
            return (str != null && str.Length > 0);
        }

        public static string TrimStart(this string str, char c)
        {
            return str.TrimStart(new[] { c });
        }
    }
}