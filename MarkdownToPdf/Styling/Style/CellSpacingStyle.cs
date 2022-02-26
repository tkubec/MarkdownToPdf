// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

namespace Orionsoft.MarkdownToPdfLib.Styling
{
    /// <summary>
    /// Part of <see cref="CascadingStyle"/> - Cell spacing of a table cell
    /// </summary>
    public class CellSpacingStyle : BoxStyle<CellSpacingStyle>
    {
        internal CellSpacingStyle()
        { }

        internal CellSpacingStyle(Dimension size) : base(size)
        {
        }

        internal CellSpacingStyle Clone()
        {
            return new CellSpacingStyle { Bottom = Bottom, Top = Top, Left = Left, Right = Right };
        }

        internal CellSpacingStyle MergeWith(CellSpacingStyle baseStyle)
        {
            var res = new CellSpacingStyle();
            ApplyTo(baseStyle, res);
            return res;
        }

        /// <summary>
        /// Sets all sides size in points
        /// </summary>
        public static implicit operator CellSpacingStyle(double size) => new CellSpacingStyle(size);

        /// <summary>
        /// Sets all sides size
        /// </summary>
        public static implicit operator CellSpacingStyle(Dimension size) => new CellSpacingStyle(size);

        /// <summary>
        /// Sets all sides size
        /// </summary>

        public static implicit operator CellSpacingStyle(string size) => new CellSpacingStyle(Dimension.Parse(size));
    }
}