# Special fields

Special fields in the markdown document serve to either insert a special content or to affect the conversion process. They are all formatted as an empty link with url starting with md, eg. `[](md:page)`.


## Block fields

Block fields must be the only content of a paragraph.

| Field        | Meaning                                   |
| ------------ | ----------------------------------------- |
| pageBreak    | ends current page and starts a new one    |
| sectionBreak | ends current section and starts a new one |


## Inline fields

| Field | Meaning                    |
| ----- | -------------------------- |
| page  | number of current page     |
| pages | total number of pages      |
| #     | anchor for cross-reference |
