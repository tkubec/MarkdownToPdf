// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Orionsoft.MarkdownToPdfLib.Styling;
using MigraDoc.DocumentObjectModel;

namespace Orionsoft.MarkdownToPdfLib.Converters
{
    public class InheritedData
    {
        public FontStyle Font { get; set; }
        public Color Background { get; set; }
        public MarginStyle Margin { get; set; }
        public PaddingStyle Padding { get; set; }
        
        /// <summary>
        /// Position of the block in Parent container
        /// </summary>
        public int BlockIndex { get; set; }

        /// <summary>
        /// Count of blocks in Parent container
        /// </summary>
        public int BlockCount { get; set; }

        public bool BlockIsFirst { get => BlockIndex == 0; }
        public bool BlockIsLast { get => BlockIndex == (BlockCount - 1); }

        public InheritedData()
        {
            Font = new FontStyle();
            Margin = new MarginStyle();
            Padding = new PaddingStyle();
        }

        public InheritedData(InheritedData original)
        {
            Margin = original.Margin.Clone();
            Padding = original.Padding.Clone();
            Font = original.Font.Clone();
            Background = original.Background;

            BlockIndex = original.BlockIndex;
            BlockCount = original.BlockCount;
        }
    }
}