# Attributes

Attributes are special fields in the markdown document, that can affect the styling or conversion process. As the markdown readability is important, they should be used sparingly.

An attribute field is enclosed in curly brackets and can contain a style name, an element id and a key or key=value pairs, all separated by space, like this `{.styleName #idName align=right}`.

For element types, see [Element reference](elements.md).

## Leaf block attributes

Leaf block attributes can be on a line immediately preceding the block, or follow the end of block, **separated by a space**:

```
{.myStyle}
Paragraph text.
```

or 
```
Paragraph text. {.myStyle}
```

## Container block attributes

Container block attributes can be on a line immediately preceding the block:

```
{.myStyle}
- list item
```

## Inline attributes

Inline attributes can follow the end of an inline element, **without a space**:

```
![link](url){.myStyle}
```

