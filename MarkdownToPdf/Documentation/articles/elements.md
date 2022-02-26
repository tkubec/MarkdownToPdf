# Element Reference

There are three basic categories of markdown elements:

- **Container blocks** - blocks containing other blocks, e.g. lists

- **Leaf blocks** - blocks of text that do not contain other blocks, only inline elements e.g.,  paragraph

- **Inlines** - plain or formatted text spans and images within a leaf block


All bock elements can have style and id attributes, e.g. `{.myStyle}` or `{#myId}`. Inline elements can have attributes only if explicitly stated so.

## Container blocks

### List

**Types**: `UnorderedList`, `OrderedList`

**Syntax**: none, defined by the list items


**Default style name**:  `UnorderedList`, `OrderedList`

**Applicable style parts**: Font, Background, Margin, Padding

#### Description

A block containing list items
**Attributes**:


### List Item

**Types**: `UnorderedListItem`, `OrderedListItem`

**Syntax**:

```markdown
- item
- item

1. item
2. item

```

**Default style name**: `UnorderedListItem`, `OrderedListItem`

**Applicable style parts**: Font, Background, Margin, Padding

#### Description

A list item containing other blocks. Either preceded by a bullet or by a number.

**Attributes**: none


### Quote

**Types**: `Quote`

**Syntax**:

```markdown
> Quote
```

**Default style name**: `Quote`, 

**Applicable style parts**: Font, Background, Margin, Padding

#### Description

A block styled as a quote containing other blocks.

**Attributes**: none


### Table

**Types**: `Table`

**Syntax**:

```markdown
| Col 1 | Col 2 |
| ----- | ----: |
| data  |  data |


+---+---+---+
| AAAAA | B |
+---+---+---+
```

**Default style name**: `Table`, 

**Applicable style parts**: Font, Background, Margin, Table, Border (only the entire border, not side by side)

#### Description

A table containing table rows.

If a table width is defined, the column widths are scaled to fit into the table width.

If a table width is not defined, the width is a sum of column widths. If it exceeds the maximum available width, column widths are scaled to fit into the maximum width.

**Attributes**: 

| Attribute | Values                                                | Note                       |
| --------- | ----------------------------------------------------- | -------------------------- |
| width     | width expressed in `Dimension` units                  |                            |
| columns   | comma separated widths expressed in `Dimension` units |                            |
| align     | left, right, center                                   | horizontal table alignment |


### Table Row

**Types**: `TableHeader`, `TableRowEven`, `TableRowOdd`

**Syntax**: see Table

**Default style name**: `TableHeader`, `TableRowEven`, `TableRowOdd`

**Applicable style parts**: Font, Background

#### Description

A table row.


**Attributes**: none


### Table Cell

**Types**: `TableCell`

**Syntax**: see Table

**Default style name**: `TableCell`

**Applicable style parts**: Font, Background, Table.VerticalCellAlignment

#### Description

A table cell.


**Attributes**: none


### Footnote Group

**Types**: `FootnoteGroup`

**Syntax**: -

**Default style name**: `FootnoteGroup`

**Applicable style parts**: Font, Background

#### Description

A block of footnotes


**Attributes**: none


### Footnote 

**Types**: `Footnote`

**Syntax**: 

```markdown
[^first]: Footnote
```

**Default style name**: `Footnote`

**Applicable style parts**: Font, Background, Bullet

#### Description

A block containing one footnote, preceded by a numbering bullet


**Attributes**: none

### Custom Container 

**Types**: `CustomContainer`

**Syntax**: 

```markdown
:::
content
:::
```
**Default style name**: `CustomContainer`

**Applicable style parts**: Font, Background

#### Description

A block containing other blocks


**Attributes**: none

## Leaf  Blocks

### Paragraph

**Types**: `Paragraph`

**Syntax**: 

```markdown
Just a text paragraph.

```

**Default style name**: `Paragraph`

**Applicable style parts**: Font, Background, Margin, Padding, Border, Paragraph

#### Description

A block containing other blocks

**Attributes**: 

| Attribute | Values                         | Note                |
| --------- | ------------------------------ | ------------------- |
| align     | left, right, center, justified | paragraph alignment |

### Heading

**Types**: `Heading1` - `Heading6`

**Syntax**: 
```markdown
# Heading 1
## Heading 2
...
###### Heading 6
```

**Default style name**: `Heading1` - `Heading6`

**Applicable style parts**: Font, Background, Margin, Padding, Border, Paragraph

#### Description

A heading.

**Attributes**: 

| Attribute | Values                         | Note                                                             |
| --------- | ------------------------------ | ---------------------------------------------------------------- |
| align     | left, right, center, justified | heading alignment                                                |
| outline   | true, false                    | if false, the heading is excluded from PDF document tree outline |

### Code

**Types**: `Code`

**Syntax**: 
```markdown
    ```optional_syntax_name
    code();
    ```

    ....code(); // 4 spaces before code


```

**Default style name**: `Code`

**Applicable style parts**: Font, Background, Margin, Padding, Border, Paragraph

#### Description

A block of code.

**Attributes**: none

### Horizontal Rule (Thematic Break)

**Types**: `Break`

**Syntax**: 
```markdown
---

***
___
```

**Default style name**: `Break`

**Applicable style parts**: Background, Margin, Padding, Border, Paragraph, Bullet

#### Description

A paragraph representing a horizontal divider.

**Attributes**: none

## Inline Elements

### Emphasis

**Types**: `Bold`, `Italic`, `Superscript`, `Subscript`, `Marked`, `Cite`, `Strike`, `Inserted`

**Syntax**: 
```markdown
**Bold**
__Bold__

*Italic*
_Italic_

^SuperScript^

~Subscript~

==Marked text==

""Citation""

~~strike~~

++Inserted text++
```

**Default style name**:  `Bold`, `Italic`, `Superscript`, `Subscript`, `Marked`, `Cite`, `Strike`, `Inserted`

**Applicable style parts**: Font

#### Description

A text styling/emphasis.

**Attributes**: none

### Inline Code

**Types**: `InlineCode`

**Syntax**: 
```markdown
`code`
```

**Default style name**:  `InlineCode`

**Applicable style parts**: Font

#### Description

A text span representing a code snippet.

**Attributes**:

| Attribute | Values              | Note                                                                    |
| --------- | ------------------- | ----------------------------------------------------------------------- |
| .style     | language         | Syntax highlighting. Although it is not exactly a style, it is used this way for compatibility reasons with other engines, e.g. `{.python}`


### Hyperlink

**Types**: `Hyperlink`

**Syntax**: 
```markdown
[Description](url)
```

**Default style name**:  `Hyperlink`

**Applicable style parts**: Font

#### Description

A hyperlink to a cross reference (with # prefix) or to a url.

**Attributes**: none






### Image

**Types**: `Hyperlink`

**Syntax**: 
```markdown
![Optional Description](url)
```

**Default style name**:  `Hyperlink`

**Applicable style parts**: -

#### Description

A raster image or embedded PDF.

**Attributes**:

| Attribute | Values              | Note                                                                    |
| --------- | ------------------- | ----------------------------------------------------------------------- |
| width     | `Dimension`         | image width                                                             |
| height    | `Dimension`         | image height                                                            |
| dpi       | number              | enforced DPI to be used                                                 |
| align     | left, right, center | image alignment (must be alone in paragraph)                            |
| watermark |                     | image is rendered below the following text (must be alone in paragraph) |


### Footnote Reference,


**Types**: `Footnote Reference`

**Syntax**: 
```markdown
Footnote 1 link[^first].
```

**Default style name**:  `Footnote Reference`

**Applicable style parts**: Font

#### Description

A reference to a footnote.

**Attributes**: - 

### HTML Entity

**Types**: -

**Syntax**: 
```markdown
&copy;
```

**Default style name**:  -

**Applicable style parts**: -

#### Description

An HTML entity in form &name; or &number;

**Attributes**: none



### TOC

**Types**: `Toc1` - `Toc6`, `Index`

**Syntax**: 
```markdown
[Chapter 1](target){.Toc1}

[Chapter 1.1](target){.Toc2}

...
```

**Default style name**:  `Toc1` - `Toc6`, `Index`

**Applicable style parts**: -

#### Description

A special hyperlink converting the paragraph to a Table of Contents line, see an example in Examples

**Attributes**: `Toc1` - `Toc6`, `Index`


