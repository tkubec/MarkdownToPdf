// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using MigraDoc.DocumentObjectModel;

namespace Orionsoft.MarkdownToPdfLib.Styling
{
    /// <summary>
    /// Part of <see cref="CascadingStyle"/> defining margin of a block, ie. space between border (or padding, if there is no border) and element's parent.
    /// The background color is same as the parent's background
    /// </summary>

    public class MarginStyle : BoxStyle<MarginStyle>
    {
        internal MarginStyle()
        { }

        internal MarginStyle(Dimension size) : base(size)
        {
        }

        internal MarginStyle Clone()
        {
            return new MarginStyle { Bottom = Bottom, Top = Top, Left = Left, Right = Right };
        }

        internal MarginStyle MergeWith(MarginStyle baseStyle)
        {
            var res = new MarginStyle();
            ApplyTo(baseStyle, res);
            return res;
        }

        internal ParagraphFormat Merge(ParagraphFormat paragraphFormat, double fontSize, double containerWidth)
        {
            var res = paragraphFormat.Clone();
            res.SpaceBefore = !Top.IsEmpty ? Top.Eval(fontSize, containerWidth) : paragraphFormat.SpaceBefore;
            res.SpaceAfter = !Bottom.IsEmpty ? Bottom.Eval(fontSize, containerWidth) : paragraphFormat.SpaceAfter;
            res.LeftIndent = !Left.IsEmpty ? Left.Eval(fontSize, containerWidth) : paragraphFormat.LeftIndent;
            res.RightIndent = !Right.IsEmpty ? Right.Eval(fontSize, containerWidth) : paragraphFormat.RightIndent;
            return res;
        }

        /// <summary>
        /// Sets all sides size in points
        /// </summary>
        public static implicit operator MarginStyle(double size) => new MarginStyle(size);

        /// <summary>
        /// Sets all sides size
        /// </summary>
        public static implicit operator MarginStyle(Dimension size) => new MarginStyle(size);

        /// <summary>
        /// Sets all sides size
        /// </summary>

        public static implicit operator MarginStyle(string size) => new MarginStyle(Dimension.Parse(size));
    }
}