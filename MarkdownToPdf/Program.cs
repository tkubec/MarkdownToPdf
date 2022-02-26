using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownToPdf
{
    class Program
    {
        static void Main(string[] args)
        {
            //BuildCookBook();
            BuildTest();
        }

        private static void BuildTest()
        {
            var txt = File.ReadAllText("../../testmd/extensions.md");
            var convertor = new MarkdownToPdf();
            var section = convertor.Document.AddSection();
            convertor.Add2(convertor.Document.LastSection,txt);
            Config(convertor);
            convertor.Save("out.pdf");
        }

        private static void BuildCookBook()
        {
            var txt = File.ReadAllText("toc.md");

            var convertor = new MarkdownToPdf();
            Config(convertor);

            var section = convertor.Document.AddSection();
            convertor.Add("![](cover.png){width=16cm}\r\n\r\n");

            section = convertor.Document.AddSection();
            convertor.Add(txt);
            section.PageSetup.StartingNumber = 1;

            txt = "{align=right}\r\n[](md:page)";
            convertor.Add(section.Footers.Primary, txt);

            txt = "{align=left}\r\n[](md:page)";
            convertor.Add(section.Footers.EvenPage, txt);

            convertor.Save("test.pdf");
        }

        private static void Config(MarkdownToPdf convertor)
        {
            convertor.FontResolver.Dir = @"..\..\font\";
            convertor.Document.DefaultPageSetup.LeftMargin = Unit.FromCentimeter(2.75);
            convertor.Document.DefaultPageSetup.RightMargin = Unit.FromCentimeter(2.25);
            convertor.Document.DefaultPageSetup.TopMargin = Unit.FromCentimeter(2.5);
            convertor.Document.DefaultPageSetup.BottomMargin = Unit.FromCentimeter(3);
            convertor.Document.DefaultPageSetup.MirrorMargins = true;
            convertor.Document.DefaultPageSetup.OddAndEvenPagesHeaderFooter = true;
        }
    }
}
