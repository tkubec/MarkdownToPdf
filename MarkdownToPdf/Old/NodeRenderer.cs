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
        private MigraDocBlockContainer adapter;
        private string rawText;

        public string StylePrefix { get; set; }

        public NodeRenderer(MigraDocBlockContainer adapter, string rawText)
        {
            this.adapter = adapter;
            this.rawText = rawText;
        }

        public void Add(MarkdownDocument md)
        {
            AddBlocks(md);
        }
        
        public void AddBlocks(IEnumerable<Block> blocks)
        {
            foreach (var block in blocks)
            {
                AddBlock(block);
            }
        }


        #region Special commands
        private void ExecuteInlineCommand(Inline inline, MigrDocInlineContainer par, bool standalone)
        {
            var cmd = MarkdigTreeHelper.ParseCommand(inline);
            if (cmd == null)
            {
                Console.WriteLine("Unknown command");
                return;
            }

            if (standalone)
            {
                switch (cmd[0].Key.ToLower())
                {
                    case "pagebreak":
                        {
                            adapter.AddPageBreak();
                            break;
                        }
                    case "sectionbreak":
                        {
                            adapter.AddSectionBreak();
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Unknown command " + cmd);
                            break;
                        }
                }
            }
            else
            {
                switch (cmd[0].Key.ToLower())
                {
                    case "page":
                        {
                            par.AddPageField();
                            break;
                        }
                    case "pages":
                        {
                            par.AddNumPagesField();
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Unknown command " + cmd);
                            break;
                        }
                }
            }
        }
        #endregion
    }
}