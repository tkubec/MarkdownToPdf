// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using MigraDoc.DocumentObjectModel;
using System.Globalization;

namespace Orionsoft.MarkdownToPdfLib
{
    public static class MigraDocExtensions
    {
        public static Unit ParseUnitEx(string text, MigrDocInlineContainer par)
        {
            if (text == null) return Unit.Empty;
            if (text.EndsWith("cm") || text.EndsWith("mm") || text.EndsWith("in"))
            {
                try
                {
                    return Unit.Parse(text);
                }
                catch { }
            }

            if (text.EndsWith("em"))
            {
                try
                {
                    var em = double.Parse(text.Substring(0, text.Length - 2), CultureInfo.InvariantCulture);

                    var fs = par.Font.Size;
                    if (fs.IsEmpty)
                    {
                        var style = par.Document.Styles[par.Style];
                        if (style != null)
                        {
                            fs = style.Font.Size;
                            if (fs.IsEmpty)
                            {
                                style = style.GetBaseStyle();
                                if (style != null)
                                {
                                    fs = style.Font.Size;
                                }
                            }
                        }
                    }
                    return fs * em;
                }
                catch { }
            }

            if (text.EndsWith("%"))
            {
                try
                {
                    var pct = double.Parse(text.Substring(0, text.Length - 1), CultureInfo.InvariantCulture);

                    var pw = par.Section.PageSetup.PageWidth;
                    pw = pw.IsEmpty ? par.Document.DefaultPageSetup.PageWidth : pw;

                    var lm = par.Section.PageSetup.LeftMargin;
                    lm = lm.IsEmpty ? par.Document.DefaultPageSetup.LeftMargin : lm;

                    var rm = par.Section.PageSetup.RightMargin;
                    rm = rm.IsEmpty ? par.Document.DefaultPageSetup.RightMargin : rm;

                    return (pw - rm - lm) * pct / 100.0;
                }
                catch { }
            }
            if (int.TryParse(text, out int res)) return res;
            return Unit.Empty;
        }

        public static Unit GetFontSize(this Style s)
        {
            if (s.Font.Size.IsEmpty)
            {
                if (s.GetBaseStyle() != null)
                {
                    return s.GetBaseStyle().GetFontSize();
                }
                return Unit.Empty;
            }
            return s.Font.Size;
        }
    }
}