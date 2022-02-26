// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

namespace Orionsoft.MarkdownToPdfLib.Styling
{
    /// <summary>
    /// Supported markdown element types
    /// </summary>
    public enum ElementType
    {
        /// <summary> Do not use directly</summary>
        Any,

        // Containers

        /// <summary> Do not use directly</summary>
        Root,

        /// <summary> List container containing other lists and list items</summary>
        UnorderedList,

        /// <summary> List container containing other lists and list items</summary>
        OrderedList,

        /// <summary> List Item container containing blocks</summary>
        UnorderedListItem,

        /// <summary> List Item container containing blocks</summary>
        OrderedListItem,

        /// <summary> Quote container containing blocks</summary>
        Quote,

        /// <summary> Container containing footnote blocks</summary>
        FootnoteGroup,

        /// <summary> Footnote container containing blocks</summary>
        Footnote,

        /// <summary> Table container containing table rows</summary>
        Table,

        /// <summary> Table heading row container containing table cells</summary>
        TableHeader,

        /// <summary> Table even row container containing table cells</summary>
        TableRowEven,

        /// <summary> Table odd row container containing table cells</summary>
        TableRowOdd,

        /// <summary> Table cell containing blocks</summary>
        TableCell,

        /// <summary> Container containing blocks</summary>
        CustomContainer,

        // Leafblocks

        /// <summary> Paragraph block</summary>
        Paragraph,

        /// <summary> Heading block</summary>
        Heading1,

        /// <summary> Heading block</summary>
        Heading2,

        /// <summary> Heading block</summary>
        Heading3,

        /// <summary> Heading block</summary>
        Heading4,

        /// <summary> Heading block</summary>
        Heading5,

        /// <summary> Heading block</summary>
        Heading6,

        /// <summary> Code block</summary>
        Code,

        /// <summary> Thematic break (horizontal rule) block</summary>
        Break,

        /// <summary>
        /// Image as an only element in paragraph
        /// </summary>
        Image,

        /// <summary>
        ///  Plugin as an only element in paragraph
        /// </summary>
        Plugin,

        // Inlines

        /// <summary>Inline emphasis</summary>
        Bold,

        /// <summary>Inline emphasis</summary>
        Italic,

        /// <summary>Inline hyperlink</summary>
        Hyperlink,

        /// <summary>Inline emphasis</summary>
        InlineCode,

        /// <summary>Inline footnote reference number</summary>
        FootnoteReference,

        /// <summary>Inline emphasis</summary>
        Superscript,

        /// <summary>Inline emphasis</summary>
        Subscript,

        /// <summary>Inline emphasis</summary>
        Cite,

        /// <summary>Inline emphasis</summary>
        Strike,

        /// <summary>Inline emphasis</summary>
        Inserted,

        /// <summary>Inline emphasis</summary>
        Marked,

        /// <summary>
        /// Inline image
        /// </summary>
        InlineImage,

        /// <summary>
        ///  Inline plugin
        /// </summary>
        InlinepPlugin,

        // Special inlines

        /// <summary>Inline hyperlink, turns line into TOC item</summary>
        Toc1,

        /// <summary>Inline hyperlink, turns line into TOC item</summary>
        Toc2,

        /// <summary>Inline hyperlink, turns line into TOC item</summary>
        Toc3,

        /// <summary>Inline hyperlink, turns line into TOC item</summary>
        Toc4,

        /// <summary>Inline hyperlink, turns line into TOC item</summary>
        Toc5,

        /// <summary>Inline hyperlink, turns line into TOC item</summary>
        Toc6,

        /// <summary>Inline hyperlink, turns line into TOC item</summary>
        Index
    }
}