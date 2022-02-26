// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig.Syntax;
using Orionsoft.MarkdownToPdfLib.Styling;
using System.Linq;
using System.Text.RegularExpressions;

namespace Orionsoft.MarkdownToPdfLib.Converters
{
    internal class RootBlockConvertor : ContainerBlockConverter, IStandaloneContainerConverter
    {
        public RootBlockConvertor(MarkdownDocument block, string rawText, MigraDocBlockContainer output,
            ContainerBlockConverter parent, MarkdownToPdf owner)
             : base(block, parent)
        {
            Owner = owner;
            RawText = rawText;
            OutputContainer = output;
            ElementDescriptor = new SingleElementDescriptor { Attributes = Attributes, Type = ElementType.Root };
            Width = Owner.RealPageWidth;
            Split(rawText);
        }

        private void Split(string rawText)
        {
            var matches = Regex.Matches(rawText, "^.*(\r\n|\r|\n)", RegexOptions.Multiline);
            Lines = matches.Cast<Match>().Select(match => match.Value).ToList();
        }

        protected override void PrepareStyling()
        {
            base.PrepareStyling();
            Width = Owner.RealPageWidth;
            FontSize = EvaluatedStyle.Font.Size.Eval(12, Owner.RealPageWidth);
        }
    }
}