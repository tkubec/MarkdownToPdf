// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using System;

namespace Orionsoft.MarkdownToPdfLib
{
    /// <summary>
    /// Provides unified interface to access MigraDoc section, header, footer or table cell
    /// </summary>
    public class MigraDocBlockContainer
    {
        /// <summary>
        /// Provides access to  underlying MigraDoc object. Null if the underlying object is of other type
        /// </summary>
        public Section Section { get; private set; }

        /// <summary>
        /// MarkdownToPdf class  owning this container
        /// </summary>
        public MarkdownToPdf Owner { get; }

        /// <summary>
        /// Provides access to  underlying MigraDoc object. Null if the underlying object is of other type
        /// </summary>
        public Cell Cell { get; private set; }

        /// <summary>
        /// Provides access to  underlying MigraDoc object. Null if the underlying object is of other type
        /// </summary>
        public HeaderFooter HeaderFooter { get; private set; }

        /// <summary>
        /// Returns type of underlying MigraDoc object
        /// </summary>
        public Type Type
        {
            get => Section != null ? typeof(Section)
                    : (Cell != null ? typeof(Cell) : null);
        }

        internal MigraDocBlockContainer(Section section, MarkdownToPdf owner)
        {
            this.Section = section;
            this.Owner = owner;
        }

        internal MigraDocBlockContainer(Cell cell, MarkdownToPdf owner)
        {
            this.Cell = cell;
            this.Owner = owner;
        }

        internal MigraDocBlockContainer(HeaderFooter headerFooter, MarkdownToPdf owner)
        {
            this.HeaderFooter = headerFooter;
            this.Owner = owner;
        }

        //
        // Summary:
        //     Adds a new paragraph with the specified text to the section.
        public MigrDocInlineContainer AddParagraph(string paragraphText)
        {
            if (Section != null) return new MigrDocInlineContainer(Section.AddParagraph(paragraphText));
            if (Cell != null) return new MigrDocInlineContainer(Cell.AddParagraph(paragraphText));
            if (HeaderFooter != null) return new MigrDocInlineContainer(HeaderFooter.AddParagraph(paragraphText));
            return null;
        }

        //
        // Summary:
        //     Adds a new paragraph to the section.
        public MigrDocInlineContainer AddParagraph()
        {
            if (Section != null) return new MigrDocInlineContainer(Section.AddParagraph());
            if (Cell != null) return new MigrDocInlineContainer(Cell.AddParagraph());
            if (HeaderFooter != null) return new MigrDocInlineContainer(HeaderFooter.AddParagraph());
            return null;
        }

        // Summary:
        //     Adds a new table to the section.
        public Table AddTable()
        {
            if (Section != null) return Section.AddTable();
            if (HeaderFooter != null) return HeaderFooter.AddTable();
            return null;
        }

        //
        // Summary:
        //     Adds a manual page break.
        //
        public void AddPageBreak()
        {
            if (Section != null) Section.AddPageBreak();
        }

        public void AddSectionBreak()
        {
            if (Section != null)
            {
                Section = Section.Document.AddSection();

                // this fixes MigraDoc bug. Unless PageSetup.PageWidth is read, the page numbering fails
                if (Owner.MigraDocument.LastSection.PageSetup.PageWidth < 0)
                {
                    Owner.OnWarningIssued(this, "SectionBreak", "negative>");
                }
            }
        }
    }
}