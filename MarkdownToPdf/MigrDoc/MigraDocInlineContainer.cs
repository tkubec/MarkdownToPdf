// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using System;

namespace Orionsoft.MarkdownToPdfLib
{
    /// <summary>
    /// Provides unified interface to access MigraDoc Paragraph and Hyperlink
    /// </summary>
    public class MigrDocInlineContainer
    {
        /// <summary>
        /// Provides access to  underlying MigraDoc object. Null if the underlying object is of other type
        /// </summary>
        public Paragraph Paragraph { get; private set; }

        /// <summary>
        /// Provides access to  underlying MigraDoc object. Null if the underlying object is of other type
        /// </summary>
        public Hyperlink Hyperlink { get; private set; }

        internal Document Document { get => GetDocument(); }
        internal Section Section { get => GetSection(); }

        /// <summary>
        /// Returns type of underlying MigraDoc object
        /// </summary>
        public Type Type
        {
            get => Paragraph != null ? typeof(Paragraph)
                    : Hyperlink != null ? typeof(Hyperlink)
                    : null;
        }

        internal MigrDocInlineContainer(Paragraph p)
        {
            this.Paragraph = p;
        }

        internal MigrDocInlineContainer(Hyperlink h)
        {
            this.Hyperlink = h;
        }

        public ParagraphFormat Format { get => GetFormat(); set => SetFormat(value); }

        private Document GetDocument()
        {
            if (Paragraph != null) return Paragraph.Document;
            if (Hyperlink != null) return Hyperlink.Document;
            return null;
        }

        private Section GetSection()
        {
            if (Paragraph != null) return Paragraph.Section;
            if (Hyperlink != null) return Hyperlink.Section;
            return null;
        }

        private void SetFormat(ParagraphFormat value)
        {
            if (Paragraph != null) Paragraph.Format = value.Clone();
        }

        private ParagraphFormat GetFormat()
        {
            if (Paragraph != null) return Paragraph.Format;
            return null;
        }

        public Font Font { get => GetFont(); set => SetFont(value); }

        private void SetFont(Font value)
        {
            if (Paragraph != null) Paragraph.Format.Font = value;
            else if (Hyperlink != null) Hyperlink.Font = value;
        }

        private Font GetFont()
        {
            if (Paragraph != null) return Paragraph.Format.Font;
            if (Hyperlink != null) return Hyperlink.Font;
            return null;
        }

        public void AddPageField()
        {
            if (Paragraph != null) Paragraph.AddPageField();
            if (Hyperlink != null) Hyperlink.AddPageField();
        }

        public void AddNumPagesField()
        {
            if (Paragraph != null) Paragraph.AddNumPagesField();
            if (Hyperlink != null) Hyperlink.AddNumPagesField();
        }

        public FormattedText AddFormattedText(string v, Font f)
        {
            if (Paragraph != null) return Paragraph.AddFormattedText(v, f);
            else if (Hyperlink != null) return Hyperlink.AddFormattedText(v, f);
            return null;
        }

        public Text AddText(string v)
        {
            if (Paragraph != null) return Paragraph.AddText(v);
            else if (Hyperlink != null) return Hyperlink.AddText(v);
            return null;
        }

        public void AddSpace(int count)
        {
            if (Paragraph != null) Paragraph.AddSpace(count);
            else if (Hyperlink != null) Hyperlink.AddSpace(count);
        }

        public void AddLineBreak()
        {
            if (Paragraph != null) Paragraph.AddLineBreak();
        }

        public Image AddImage(string v)
        {
            if (Paragraph != null) return Paragraph.AddImage(v);
            if (Hyperlink != null) return Hyperlink.AddImage(v);
            return null;
        }

        public void AddBookmark(string id)
        {
            if (Paragraph != null) Paragraph.AddBookmark(id);
            else if (Hyperlink != null) Hyperlink.AddBookmark(id);
        }

        public MigrDocInlineContainer AddHyperlink(string v, HyperlinkType hyperlinkType)
        {
            if (Paragraph != null) return new MigrDocInlineContainer(Paragraph.AddHyperlink(v, hyperlinkType));
            return null;
        }

        public void AddPageRefField(string v)
        {
            if (Paragraph != null) Paragraph.AddPageRefField(v);
            else if (Hyperlink != null) Hyperlink.AddPageRefField(v);
        }
    }
}