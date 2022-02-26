# Standard Features Overview

This is a brief overview of supported markdown features. Usage of attributes is described in another document - `attributes.md`.

## Headings

# H1 Heading 
## H2 Heading
### H3 Heading
#### H4 Heading
##### H5 Heading
###### H6 Heading

## Paragraphs 

Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam vel auctor diam. Maecenas quis lacus eget tortor aliquam accumsan commodo ut massa. Donec quis egestas augue. Aliquam cursus magna eu odio euismod, maximus faucibus nibh lacinia. Donec diam nisi, egestas ut nunc id, fermentum congue magna. Nam dignissim lacinia diam. Integer dapibus justo orci, sit amet scelerisque arcu vehicula tincidunt. 

Paragraphs can contain line breaks. To break a line, either append multiple spaces:  
or a backslash: \
or `<br/>`: <br />
It works all the same way.

## Horizontal Rules

---

## Lists

### Unordered lists

 - Item 1
 - Item 2
    - Subitem 1
    - Subitem 2. Any Item may consist of multiple lines.

      Like this...

### Ordered lists

1. Lorem ipsum dolor sit amet
2. Consectetur adipiscing elit
3. Integer molestie lorem at massa


A list can start with an offset and other numbers need not be in order, the items are numbered incrementally:

57. foo
1. bar

## Blockquotes

> Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam vel auctor diam. Maecenas quis lacus eget tortor aliquam accumsan commodo ut massa. Donec quis egestas augue. Aliquam cursus magna eu odio euismod, maximus faucibus nibh lacinia. Donec diam nisi, egestas ut nunc id, fermentum congue magna. Nam dignissim lacinia diam. Integer dapibus justo orci, sit amet scelerisque arcu vehicula tincidunt. 
>
>
> Blockquotes can also be nested
>>  like this


## Indented code - github style

    // Some comments
    line 1 of code
    line 2 of code
    line 3 of code


## Emphasis

**This is bold text**

__This is bold text__

*This is italic text*

_This is italic text_

**This is bold text**

__This is bold text__

*This is italic text*

This is **bold and *italic* text**

Inline `code` text

## Links

[link to Google](http://www.google.com)


Autoconverted link http://www.google.com is also possible


## Images

![Clock](../../data/images/clock.png)


# Extended features

## Block code "fences"

```python
num = int(input("Enter a number: "))    # user input
factorial = 1    
if num < 0:    
    print(" Factorial does not exist for negative numbers")    
elif num == 0:    
    print("The factorial of 0 is 1")    
else:    
    for i in range(1, num + 1):    
        factorial = factorial * i    
    print("The factorial of", num, "is", factorial)    
```

## Tables


| Option | Description |
| ------ | ----------- |
| data   | path to data files to supply the data that will be passed into templates. |
| engine | engine to be used for processing templates. Handlebars is the default. |
| ext    | extension to be used for dest files. |

Right aligned column

| City        | Country | Population |
| ----------- | ------- | ---------: |
| Tōkyō       | Japan   | 37,400,068 |
| Delhi       | India   | 28,514,000 |
| Shanghai    | China   | 25,582,000 |
| São Paulo   | Brazil  | 21,650,000 |
| Mexico City | Mexico  | 21,581,000 |
| Cairo       | Egypt   | 20,076,000 |
| Mumbai      | India   | 19,980,000 |

### Column and row spans

+---+---+---+
| AAAAA | B |
+---+---+ B +
| D | E | B |
+ D +---+---+
| D | CCCCC |
+---+---+---+

## Checklists

- [ ] Item1
- [x] Item2
- [ ] Item3

## Custom bulets

{.custom}
- Item1
- Item2
- Item3


## Superscript and subscript

Superscript: 1^st^ and subscript: H~2~SO~4~.

## Smartypants

Smartypants enable automatic typographic conversion of quotes, dashes, etc. : "double quotes", 'single quotes' and elipsis ...

## Emphasis

~~Strikethrough~~


++Inserted text++

==Marked text==

""Citation""

## HTML entities

`&copy;`: &copy;,  `&trade;`: &trade;, `&nbsp;` non-breakable space, etc..

## Special fields

Pagenumber: `[](md:page)` - [](md:page)

Cout of pages in the document: `[](md:pages)` - [](md:pages)

Pagebreak and section break ... `[](md:pagebreak)` `[](md:sectionbreak)`

[](md:pagebreak)

## Anchors

A block element with `id` attribute (e.g. `{#myId}`) can be cross-referenced. It is also possible to insert a stand-alone anchor:

- `[](md:#myAnchor)` [](md:#myAnchor)
- `<a id="myAnchor">`
- `<a name="myAnchor">`

But the HTML anchor must not be at the very beginning of the line, otherwise the entire line is expected to be an HTML block and such as skipped.

This is [Link to my anchor](#myAnchor)


## Footnotes

Footnote 1 link[^first].

Footnote 2 link[^second].


Duplicated footnote reference[^second].

[^first]: Footnote **can have markup**

    and multiple paragraphs.

[^second]: Footnote text.


## Custom containers

::::
Custom containers can be even nested, if necessary.
:::
like this
:::
and back...
::::

## Footnotes