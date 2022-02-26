# Styles

Styling an element via a style requires two steps:

1. Defining of a style
   
2. Binding the style to markdown elements according to their type, name, properties and position in the document tree


## Definition of a style
A style is a collection of styling properties like color, indentation, font, etc. Not all properties can be used for all elements. In such a case, they are ignored. For details, see [Element reference](elements.md). Any style can be bound to multiple element types, but each element can use one style only (see below - the style conflicts).

A style can be created from scratch:

```csharp
var style = pdf.StyleManager.AddStyle("myStyle", MarkdownStyleNames.Break);
```

Or it can be derived from other style:

```csharp
var asterismStyle = pdf.StyleManager.AddStyle("asterismRuler", MarkdownStyleNames.Break);
```

In such a case, when the style is used, all it's parents properties are used, unless redefined by the style or any later ancestor

The style name is not mandatory, but can be useful for later access to the style definition

After the style is created, the properties can be set to define the actual styling:

```csharp
style.Font.Color = Colors.Blue;
style.Paragraph.Alignment = ParagraphAlignment.Justify;
```

## Binding a style to a markdown element

The style is bound to an element using a chain of selectors.

The simplest case is just binding to an element type:

```csharp
pdf.StyleManager.ForElement(ElementType.Paragraph).Bind(style);
```


Or, binding to an element type with style name set via an attribute {.styleName}

```csharp
pdf.StyleManager.ForElement(ElementType.Paragraph, "styleName").Bind(style);
```

To apply the style only to elements with special position in the document  tree, selectors `WithParent`, `WithAncestor` and `Where` can be used.

For example:

```csharp
pdf.StyleManager.ForElement(ElementType.UnorderedListItem).WithAncestor(ElementType.UnorderedListItem).Bind(style);
```
binds the style only to nested list items, but not to first-level lists

## Style conflicts

If multiple styles fulfil the binding conditions, the style is selected by these rules:

1. binding with explicit style name is preferred to type-only based  binding

2. binding `WithParent` is preferred to `WithAncestor`

3. more specific binding is preferred to a less specific one

