// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using MigraDoc.DocumentObjectModel;

namespace Orionsoft.MarkdownToPdfLib.Styling
{
    /// <summary>
    /// Part of <see cref="CascadingStyle"/> defining paragraph format. It reflects the MigraDoc paragraph styling, see MigraDoc for details
    /// </summary>
    ///
    public class ParagraphStyle
    {
        public ParagraphAlignment? Alignment { get; set; }
        public Dimension FirstLineIndent { get; set; }

        public Dimension LineSpacing { get; set; }
        public LineSpacingRule? LineSpacingRule { get; set; }

        public bool? PageBreakBefore { get; set; }

        public bool? KeepTogether { get; set; }

        public bool? KeepWithNext { get; set; }

        public bool? WidowControl { get; set; }

        public OutlineLevel? OutlineLevel { get; set; }

        internal ParagraphStyle()
        {
            FirstLineIndent = new Dimension();
            LineSpacing = new Dimension();
        }

        internal ParagraphFormat Merge(ParagraphFormat paragraphFormat, double fontSize, double containerWidth)
        {
            var res = paragraphFormat.Clone();

            res.Alignment = Alignment ?? paragraphFormat.Alignment;
            res.FirstLineIndent = !FirstLineIndent.IsEmpty ? FirstLineIndent.Eval(fontSize, containerWidth) : paragraphFormat.FirstLineIndent;
            res.LineSpacing = !LineSpacing.IsEmpty ? LineSpacing.Eval(fontSize, containerWidth) : paragraphFormat.LineSpacing;
            res.LineSpacingRule = LineSpacingRule ?? paragraphFormat.LineSpacingRule;
            res.PageBreakBefore = PageBreakBefore ?? paragraphFormat.PageBreakBefore;
            res.KeepTogether = KeepTogether ?? paragraphFormat.KeepTogether;
            res.KeepWithNext = KeepWithNext ?? paragraphFormat.KeepWithNext;
            res.WidowControl = WidowControl ?? paragraphFormat.WidowControl;
            res.OutlineLevel = OutlineLevel ?? paragraphFormat.OutlineLevel;

            return res;
        }

        internal ParagraphStyle MergeWith(ParagraphStyle baseStyle)
        {
            var res = baseStyle.Clone();

            res.Alignment = Alignment ?? baseStyle.Alignment;
            res.FirstLineIndent = !FirstLineIndent.IsEmpty ? FirstLineIndent : baseStyle.FirstLineIndent;
            res.LineSpacing = !LineSpacing.IsEmpty ? LineSpacing : baseStyle.LineSpacing;
            res.LineSpacingRule = LineSpacingRule ?? baseStyle.LineSpacingRule;
            res.PageBreakBefore = PageBreakBefore ?? baseStyle.PageBreakBefore;
            res.KeepTogether = KeepTogether ?? baseStyle.KeepTogether;
            res.KeepWithNext = KeepWithNext ?? baseStyle.KeepWithNext;
            res.WidowControl = WidowControl ?? baseStyle.WidowControl;
            res.OutlineLevel = OutlineLevel ?? baseStyle.OutlineLevel;

            return res;
        }

        internal ParagraphStyle Clone()
        {
            return new ParagraphStyle
            {
                FirstLineIndent = FirstLineIndent,
                Alignment = Alignment,
                LineSpacing = LineSpacing,
                LineSpacingRule = LineSpacingRule,
                PageBreakBefore = PageBreakBefore,
                KeepTogether = KeepTogether,
                KeepWithNext = KeepWithNext,
                WidowControl = WidowControl,
                OutlineLevel = OutlineLevel
            };
        }
    }
}