// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using PdfSharp.Fonts;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Orionsoft.MarkdownToPdfLib
{
    internal class FontResolver : IFontResolver
    {
        private readonly List<FontFamily> _fonts = null;
        public string Dir { get; set; }

        public FontResolver()
        {
            _fonts = new List<FontFamily>();
        }

        public FontResolver(string dir) : this()
        {
            Dir = dir ?? "";
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            Dir = Dir ?? "";
            var name = familyName.ToLower();

            var registeredFont = _fonts.FirstOrDefault(x => x.Name.ToLower() == name);

            if (registeredFont == null) return PlatformFontResolver.ResolveTypeface(familyName, isBold, isItalic);

            if (isBold)
            {
                if (isItalic)
                {
                    if (registeredFont.Normal == registeredFont.BoldItalic) return new FontResolverInfo(registeredFont.Normal, PdfSharp.Drawing.XStyleSimulations.BoldItalicSimulation);
                    if (registeredFont.Italic == registeredFont.BoldItalic) return new FontResolverInfo(registeredFont.Italic, PdfSharp.Drawing.XStyleSimulations.BoldSimulation);
                    if (registeredFont.Bold == registeredFont.BoldItalic) return new FontResolverInfo(registeredFont.Bold, PdfSharp.Drawing.XStyleSimulations.ItalicSimulation);
                    return new FontResolverInfo(registeredFont.BoldItalic);
                }

                if (registeredFont.Normal == registeredFont.Bold) return new FontResolverInfo(registeredFont.Normal, PdfSharp.Drawing.XStyleSimulations.BoldSimulation);
                return new FontResolverInfo(registeredFont.Bold);
            }
            if (isItalic)
            {
                if (registeredFont.Normal == registeredFont.Italic) return new FontResolverInfo(registeredFont.Normal, PdfSharp.Drawing.XStyleSimulations.ItalicSimulation);
                return new FontResolverInfo(registeredFont.Italic);
            }

            return new FontResolverInfo(registeredFont.Normal);
        }

        public void Register(string name, string regular, string bold = "", string italic = "", string boldItalic = "")
        {
            var existing = _fonts.FirstOrDefault(x => x.Name == name);

            if (existing != null)
            {
                _fonts.RemoveAt(_fonts.IndexOf(existing));
            }

            _fonts.Add(new FontFamily(name, regular, bold, italic, boldItalic));
        }

        public byte[] GetFont(string faceName)
        {
            if (File.Exists(faceName))
            {
                return File.ReadAllBytes(faceName);
            }
            else
            {
                if (!(Dir.EndsWith(Path.DirectorySeparatorChar.ToString()) || Dir.EndsWith(Path.AltDirectorySeparatorChar.ToString())))
                {
                    Dir += Path.DirectorySeparatorChar;
                }
                return File.ReadAllBytes(Dir + faceName);
            }
        }
    }
}