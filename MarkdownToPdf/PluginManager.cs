// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Orionsoft.MarkdownToPdfLib.Converters;
using Orionsoft.MarkdownToPdfLib.Plugins;
using System;
using System.Collections.Generic;

namespace Orionsoft.MarkdownToPdfLib
{
    /// <summary>
    /// Enables adding plugins
    /// </summary>

    public sealed class PluginManager
    {
        private readonly List<IHighlightingPlugin> highlightingPlugins = new List<IHighlightingPlugin>();
        private readonly List<IImagePlugin> imagePlugins = new List<IImagePlugin>();
        private readonly MarkdownToPdf owner;

        internal PluginManager(MarkdownToPdf owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Adds highlighting plugin. If more plugins are added and the first is unable to higlight the data, the following plugins get a chance
        /// </summary>
        public void Add(IHighlightingPlugin highlightingPlugin)
        {
            highlightingPlugins.Add(highlightingPlugin);
        }

        /// <summary>
        /// Adds a math or image  plugin. If more plugins are added and the first is unable to convertt the data, the following plugins get a chance
        /// </summary>
        public void Add(IImagePlugin imagePlugin)
        {
            imagePlugins.Add(imagePlugin);
        }

        /// <summary>
        /// Adds a math plugin and enables $math$ parsing. If more plugins are added and the first is unable to convertt the data, the following plugins get a chance
        /// </summary>
        public void AddMathPlugin(IImagePlugin imagePlugin)
        {
            imagePlugins.Add(imagePlugin);
            owner.ConversionSettings.UseMath();
        }

        internal HighlightingPluginResult Highlight(List<string> lines, IElementConverter converter)
        {
            foreach (var p in highlightingPlugins)
            {
                try
                {
                    var res = p.Convert(lines, converter);
                    if (res.Success) return res;

                    if (res.Message.HasValue()) owner.OnWarningIssued(this, "HighlightPlugin", res.Message);
                }
                catch (Exception e)
                {
                    owner.OnWarningIssued(this, "Plugin", e.Message);
                }
            }
            return null;
        }

        internal ImagePluginResult GetImage(string data, IElementConverter converter)
        {
            foreach (var p in imagePlugins)
            {
                try
                {
                    var res = p.Convert(data, converter);
                    if (res.Success)
                    {
                        owner.tempFiles.Add(res.FileName);
                        return res;
                    }

                    if (res.Message.HasValue()) owner.OnWarningIssued(this, "ImagePlugin", res.Message);
                }
                catch (Exception e)
                {
                    owner.OnWarningIssued(this, "Plugin", e.Message);
                }
            }
            return null;
        }
    }
}