// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using MigraDoc.DocumentObjectModel;

namespace Orionsoft.MarkdownToPdfLib.Styling
{
    /// <summary>
    /// Part of <see cref="CascadingStyle"/> defining padding of a block, ie. space between the element's content and border (or margin, if there is no border)
    /// The background color is same as the element's background
    /// </summary>

    public class PaddingStyle : BoxStyle<PaddingStyle>
    {
        internal PaddingStyle()
        { }

        internal PaddingStyle(Dimension size) : base(size)
        {
        }

        internal PaddingStyle Clone()
        {
            return new PaddingStyle { Bottom = Bottom, Top = Top, Left = Left, Right = Right };
        }

        internal PaddingStyle MergeWith(PaddingStyle baseStyle)
        {
            var res = new PaddingStyle();
            ApplyTo(baseStyle, res);
            return res;
        }

        internal ParagraphFormat Merge(ParagraphFormat paragraphFormat, double fontSize, double containerWidth)
        {
            var res = paragraphFormat.Clone();

            res.Borders.DistanceFromTop = !Top.IsEmpty ? Top.Eval(fontSize, containerWidth) : paragraphFormat.Borders.DistanceFromTop;
            res.Borders.DistanceFromBottom = !Bottom.IsEmpty ? Bottom.Eval(fontSize, containerWidth) : paragraphFormat.Borders.DistanceFromBottom;
            res.Borders.DistanceFromLeft = !Left.IsEmpty ? Left.Eval(fontSize, containerWidth) : paragraphFormat.Borders.DistanceFromLeft;
            res.Borders.DistanceFromRight = !Right.IsEmpty ? Right.Eval(fontSize, containerWidth) : paragraphFormat.Borders.DistanceFromRight;
            return res;
        }

        /// <summary>
        /// Sets all sides size in points
        /// </summary>
        public static implicit operator PaddingStyle(double size) => new PaddingStyle(size);

        /// <summary>
        /// Sets all sides size
        /// </summary>
        public static implicit operator PaddingStyle(Dimension size) => new PaddingStyle(size);

        /// <summary>
        /// Sets all sides size
        /// </summary>

        public static implicit operator PaddingStyle(string size) => new PaddingStyle(Dimension.Parse(size));
    }
}