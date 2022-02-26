// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig;
using Markdig.Extensions.SmartyPants;
using PdfSharp.Fonts;
using System.Collections.Generic;

namespace Orionsoft.MarkdownToPdfLib
{
    /// <summary>
    /// Additional settings for conversion performed by <see cref="MarkdownToPdf"/>
    /// </summary>
    public sealed class ConversionSettings
    {
        /// <summary>
        /// Mapping of typographic substitutions for various types of quotes, dashes and elipsis
        /// </summary>
        /// <seealso href="https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/SmartyPantsSpecs.md"/>
        public Dictionary<SmartyPantType, string> SmartyPantsMapping { get; private set; }

        /// <summary>
        /// Default path to directory with custom (non-system) fonts
        /// </summary>
        public string FontDir { get => (GlobalFontSettings.FontResolver as FontResolver).Dir; set => (GlobalFontSettings.FontResolver as FontResolver).Dir = value; }

        /// <summary>
        /// Default path to directory with images referenced from markdown text
        /// </summary>
        public string ImageDir { get; set; }

        /// <summary>
        /// If set, all images without explicit DPI settings are added with DPI set to this value
        /// </summary>
        public double DefaultDpi { get; set; }

        /// <summary>
        /// Markdig pipeline used for parsing markdown text.
        /// </summary>
        /// <seealso href="https://github.com/xoofx/markdig"/>
        public MarkdownPipeline Pipeline { get; set; }

        internal ConversionSettings()
        {
            BuildPipeline();
            SmartyPantsMapping = new Dictionary<SmartyPantType, string>();
            ImageDir = "";
        }

        internal void UseMath()
        {
            BuildPipeline(math: true);
        }

        private void BuildPipeline(bool math = false)
        {
            var pipelineBuilder = new MarkdownPipelineBuilder()
                .UseAutoIdentifiers()
                .UseCitations()
                .UseCustomContainers()
                .UseEmphasisExtras()
                .UseFootnotes()
                .UseGridTables()
                .UsePipeTables()
                .UseListExtras()
                .UseTaskLists()
                .UseAutoLinks()
                .UseSmartyPants();

            if (math) pipelineBuilder.UseMathematics();

            pipelineBuilder.UseGenericAttributes(); // Must be last as it is one parser that is modifying other parsers

            Pipeline = pipelineBuilder.Build();
        }
    }
}