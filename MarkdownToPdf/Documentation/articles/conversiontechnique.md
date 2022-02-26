# Conversion Technique

Markdown document has a tree structure. It contains blocks containing other blocks, containing inline elements. On the other hand, MigraDoc document is (except for tables) just a simple sequence of paragraphs with indentation and spacing. These paragraphs can contain inline elements.


**This brings some limitations that must be taken in account**: 

The converter must emulate the tree hierarchy as it cannot nest the output objects. It is important to realize that nested objects do not have actual margins and paddings, they are simulated as well as possible by adding dummy paragraphs, indentation and by adjusting spacings of paragraphs.  

Inline objects also cannot have background color in MigraDoc.

Table cells cannot span across multiple pages.