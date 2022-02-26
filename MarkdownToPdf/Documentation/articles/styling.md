# Styling

The styling of a markdown element is based on applied [style](styles.md) and [attributes](attributes.md) within the markdown document.

The styling is applied to any markdown element in the following order:

1. Inherited styling from parent elements, like font or background color
2. Styling based on element's style (and all styles it is derived from)
3. Attribute based styling
4. `StylingPrepared` event and `BindAndModify` styling

In every step, only properties with defined value are used, otherwise the properties from previous steps are used.