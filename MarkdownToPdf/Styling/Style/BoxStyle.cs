// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using System;

namespace Orionsoft.MarkdownToPdfLib.Styling
{
    /// <summary>
    /// Base class for Margin, Padding and Cellspacing styles describing single sides
    /// </summary>
    /// <typeparam name="TBoxStyle"></typeparam>
    public class BoxStyle<TBoxStyle>
    {
        public Dimension Top { get; set; }
        public Dimension Bottom { get; set; }
        public Dimension Left { get; set; }
        public Dimension Right { get; set; }

        /// <summary>
        /// Gets or sets a side referenced by key instead via side property name. Useful when side is passed as an argument to a method
        /// </summary>
        public Dimension this[BoxSide key]
        {
            get => GetSide(key);
            set => SetSide(key, value);
        }

        internal BoxStyle()
        {
            Top = new Dimension();
            Bottom = new Dimension();
            Left = new Dimension();
            Right = new Dimension();
        }

        internal BoxStyle(Dimension size)
        {
            Bottom = size;
            Top = size;
            Left = size;
            Right = size;
        }

        internal void ApplyTo(BoxStyle<TBoxStyle> style, BoxStyle<TBoxStyle> res)
        {
            res.Top = !Top.IsEmpty ? Top : style.Top;
            res.Bottom = !Bottom.IsEmpty ? Bottom : style.Bottom;
            res.Left = !Left.IsEmpty ? Left : style.Left;
            res.Right = !Right.IsEmpty ? Right : style.Right;
        }

        private Dimension GetSide(BoxSide key)
        {
            switch (key)
            {
                case BoxSide.Left: return Left;
                case BoxSide.Right: return Right;
                case BoxSide.Top: return Top;
                case BoxSide.Bottom: return Bottom;
            }
            throw new ArgumentException("Unknown side key");
        }

        private void SetSide(BoxSide key, Dimension value)
        {
            switch (key)
            {
                case BoxSide.Left: Left = value; return;
                case BoxSide.Right: Right = value; return;
                case BoxSide.Top: Top = value; return;
                case BoxSide.Bottom: Bottom = value; return;
            }
            throw new ArgumentException("Unknown side key");
        }
    }
}