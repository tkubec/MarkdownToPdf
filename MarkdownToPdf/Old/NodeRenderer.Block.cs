using Markdig.Extensions.Tables;
using Markdig.Extensions.GenericAttributes;
using Markdig.Extensions;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static Markdig.Syntax.CodeBlock;
using Markdig.Parsers;
using System.Text.RegularExpressions;
using MigraDoc.DocumentObjectModel.Shapes;
using System.Globalization;

namespace MarkdownToPdf
{
    public partial class NodeRenderer
    {
        #region Blocks
        public void AddBlock(Block block)
        {
            var type = block.GetType();


            if (type == typeof(ParagraphBlock))
            {
                AddParagraph(block as ParagraphBlock);
            }
            else if (type == typeof(HeadingBlock))
            {
                AddHeading(block as HeadingBlock);
            }
            else if (type == typeof(ListBlock))
            {
                AddList(block as ListBlock);
                adapter.AddParagraph();
            }
            else if (type == typeof(Table))
            {
                AddTable(block as Table);
            }
            else if (type == typeof(CodeBlock) || type == typeof(FencedCodeBlock))
            {
                AddCodeBlock(block);
            }
            else
            {
                Console.WriteLine("Ignored block: " + type.ToString());
            }
        }


        private void AddParagraph(ParagraphBlock paragraphBlock, ParagraphFormatting formatting = new ParagraphFormatting())
        {
            if (MarkdigTreeHelper.IsCommandParagraph(paragraphBlock))
            {
                ExecuteInlineCommand(paragraphBlock.Inline.FirstChild, null, standalone: true);
            }
            else
            {
                Paragraph lastChild = null;

                
                lastChild = adapter.Section?.Elements.Count > 0 ?  adapter.Section?.LastParagraph : null;
                var par = adapter.AddParagraph("");

                par.Style = MarkdownStyleNames.Paragraph;
                if (StylePrefix != null)
                {
                    par.Style = StylePrefix + MarkdownStyleNames.Paragraph;
                }

                var align = MarkdigTreeHelper.GetAttribute("align", paragraphBlock, rawText);
                switch(align)
                {
                    case "justify": par.Format.Alignment = ParagraphAlignment.Justify; break;
                    case "left": par.Format.Alignment = ParagraphAlignment.Left; break;
                    case "right": par.Format.Alignment = ParagraphAlignment.Right; break;
                    case "center": par.Format.Alignment = ParagraphAlignment.Center; break;
                }

                var style = MarkdigTreeHelper.GetStyleName(paragraphBlock, rawText);
                if (style != null)
                {
                    par.Style = style;
                    //todo tmp fix
                    if (style == "author") lastChild.Format.KeepWithNext = true;
                }
                //todo tune
                if (formatting.BulletList)
                {
                    par.Style = StyleNames.List;
                    par.Format.ListInfo.ListType = ListType.BulletList3; //todo

                }
                if (!formatting.Indent.IsEmpty)
                {
                    par.Format.LeftIndent += formatting.Indent; //todo
                    par.Format.FirstLineIndent -= 10;
                }
                //par.Format.ListInfo.ListType = ListType.BulletList3;
                //par.Format.ListInfo.NumberPosition = 0;
                //par.Format.ListInfo.ContinuePreviousList = false;

                foreach (var i in paragraphBlock.Inline)
                {
                    var iFormatting = new InlineFormatting();

                    AddInline(par, i, iFormatting);
                }

            }
        }

        private void AddHeading(HeadingBlock paragraphBlock)
        {
            var id = MarkdigTreeHelper.GetParagrapId(paragraphBlock, rawText);
            var par = adapter.AddParagraph("");
            var attr = new SpecialAttributes(MarkdigTreeHelper.GetTextAfter(paragraphBlock, rawText));
            if (attr.GetFirstValue("outline") == "false") par.Format.OutlineLevel = 0;

            if (id != null)
            {
                par.AddBookmark(id.TrimStart(new[] { '#' }));
            }
            else
            {
                // automatic bookmark - hamburger format
                var bookmark = "";

                foreach (var i in paragraphBlock.Inline)
                {
                    if (i is LiteralInline) bookmark += (i as LiteralInline).Content.ToString();
                }

                par.AddBookmark(bookmark.Replace(' ', '-'));
            }

            par.Style = "Heading" + paragraphBlock.Level;

            foreach (var i in paragraphBlock.Inline)
            {
                var formatting = new InlineFormatting();
                AddInline(par, i, formatting);
            }
        }

        private void AddList(ListBlock paragraphBlock, ParagraphFormatting formatting = new ParagraphFormatting())
        {
            formatting.Indent += 20;
            formatting.BulletList = true;

            //            var par = adapter.AddParagraph("LB");

            //par.Style = "Heading" + paragraphBlock.Level;

            foreach (var i in paragraphBlock)
            {
                var type = i.GetType();


                if (type == typeof(ParagraphBlock))
                {
                    AddParagraph(i as ParagraphBlock, formatting);
                    var p = adapter.AddParagraph();
                }
                if (type == typeof(ListBlock))
                {
                    AddList(i as ListBlock, formatting);
                }
                if (type == typeof(ListItemBlock))
                {
                    //                  var par2 = adapter.AddParagraph("LIB");
                    var lbi = i as ListItemBlock;
                    foreach (var item in lbi)
                    {
                        if (item is ParagraphBlock) AddParagraph(item as ParagraphBlock, formatting);
                        if (item is ListBlock) AddList(item as ListBlock, formatting);
                    }
                    //AddInlines(par, i, new InlineFormatting());
                }

            }
            
        }

        private void AddCodeBlock(Block block)
        {
            var lines = block is FencedCodeBlock ? (block as FencedCodeBlock).Lines : (block as CodeBlock).Lines;
            var sb = new StringBuilder();

            var cnt = 0;
            foreach (var l in lines)
            {
                if (cnt >= lines.Count) break;
                sb.AppendLine(l.ToString());
                cnt++;
            }

            var str = sb.ToString().Replace(' ', (char)0x00A0); //non breaking space
            var par = adapter.AddParagraph(str);
            par.Style = MarkdownStyleNames.Code;
        }

        private void AddTable(Table mdTable) //todo
        {
            var table = adapter.AddTable();
            var text = MarkdigTreeHelper.GetTextBefore(mdTable, rawText);
            //table.Format.SpaceAfter = Unit.FromMillimeter(8); // todo tmp
            foreach (var c in mdTable.ColumnDefinitions)
            {
                var cl = table.Columns.AddColumn();
                if (cl.Index == 0) cl.Width = Unit.FromCentimeter(2.5); // 72 * 5; //todo tmp

                //todo tmp
                if (cl.Index == 1)
                {
                    cl.Width = text.Trim() == "{wide}" ? Unit.FromCentimeter(13) : Unit.FromCentimeter(6.5); // 72 * 5; //todo tmp
                }
            }
            foreach (TableRow mdRow in mdTable)
            {
                var row = table.Rows.AddRow();
                for (var i = 0; i < mdRow.Count; i++)
                {
                    var nr = new NodeRenderer(new MigraDocBlockContainer(row.Cells[i]), rawText);
                    nr.StylePrefix = "Cell";
                    nr.AddBlocks(mdRow[i] as TableCell);
                }
            }
            table.AddRow(); // separator fixer todo
        }

        #endregion


    }
}
