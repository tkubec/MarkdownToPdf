// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Orionsoft.MarkdownToPdfLib
{
    /// <summary>
    /// Object representing either absolute or relative dimension. It can be expressed as an absolute size in points, cm, mm, etc. or as a relative size based on current font size or container width
    /// </summary>
    public class Dimension
    {
        private readonly List<(DimensionUnit Unit, double Value)> content;

        public bool IsEmpty { get => !content.Any(); }

        public Dimension()
        {
            content = new List<(DimensionUnit Unit, double Value)>();
        }

        internal Dimension(DimensionUnit unit, double value) : this()
        {
            content.Add((unit, value));
        }

        private Dimension(List<(DimensionUnit Unit, double Value)> content)
        {
            this.content = content;
        }

        /// <summary>
        /// Creates Dimension based on an absolute value. Point = 1/72 inch
        /// </summary>
        public static Dimension FromPoints(double value)
        {
            return new Dimension(DimensionUnit.Point, value);
        }

        /// <summary>
        /// Creates Dimension based on an absolute value
        /// </summary>
        public static Dimension FromCentimeters(double value)
        {
            return new Dimension(DimensionUnit.Centimeter, value);
        }

        /// <summary>
        /// Creates Dimension based on an absolute value
        /// </summary>
        public static Dimension FromMillimeters(double value)
        {
            return new Dimension(DimensionUnit.Millimeter, value);
        }

        /// <summary>
        /// Creates Dimension based on an absolute value
        /// </summary>
        public static Dimension FromInches(double value)
        {
            return new Dimension(DimensionUnit.Inch, value);
        }

        /// <summary>
        /// Creates Dimension based on font size (see <see cref="Dimension.Eval(double, double)"/>)
        /// </summary>
        /// <param name="value">multiplier of current font size</param>
        public static Dimension FromFontSize(double value)
        {
            return new Dimension(DimensionUnit.FontSize, value);
        }

        /// <summary>
        /// Creates Dimension based on parent container width. The value is in percents (see <see cref="Dimension.Eval(double, double)"/>)
        /// </summary>
        /// <param name="value">Percents of width of parrent container</param>
        public static Dimension FromContainerWidth(double value)
        {
            return new Dimension(DimensionUnit.ContainerWidth, Math.Max(Math.Min(100, value), -100));
        }

        /// <summary>
        /// Converts Dimension to points, if the value is relative, size of current block font and parent container width is used for conversion
        /// </summary>
        public Unit Eval(double fontSize, double containerWidth)
        {
            if (fontSize <= 0 || containerWidth <= 0) throw new ArgumentException("Invalid arguments for Dimension.Eval()");

            if (IsEmpty) return Unit.Empty;

            var res = 0.0;
            foreach (var c in content)
            {
                switch (c.Unit)
                {
                    case DimensionUnit.Point: res += c.Value; break;

                    case DimensionUnit.Centimeter: res += c.Value / 2.54 * 72; break;

                    case DimensionUnit.Millimeter: res += c.Value / 25.4 * 72; break;

                    case DimensionUnit.Inch: res += c.Value * 72; break;

                    case DimensionUnit.FontSize: res += c.Value * fontSize; break;

                    case DimensionUnit.ContainerWidth: res += c.Value * containerWidth / 100.0; break;

                    default:
                        throw new ArgumentException("Unit evaluation error.");
                }
            }
            return res;
        }

        public static implicit operator Dimension(double value)
        {
            return FromPoints(value);
        }

        public static implicit operator Dimension(string value)
        {
            return Parse(value);
        }

        public static implicit operator Dimension(Unit value)
        {
            return FromPoints(value);
        }

        /// <summary>
        /// Creates new dimension from string representing the dimension, eg. "1.3cm"
        /// </summary>
        /// <exception cref="ArgumentException" />
        /// <param name="text">Decimal number followed by unit: cm/mm/in/pt/em/%. If no unit is specified, it is expected to be point</param>
        /// <returns></returns>
        public static Dimension Parse(string text)
        {
            var m = Regex.Match(text.Trim(), @"^(\d*(\.)?\d+)\s*(em|cm|mm|in|pt|%)?$");
            if (!m.Success) throw new ArgumentException("Invalid dimension");
            var value = double.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
            var unit = m.Groups[3].Value;

            switch (unit)
            {
                case "": return new Dimension(DimensionUnit.Point, value);
                case "pt": return new Dimension(DimensionUnit.Point, value);
                case "cm": return new Dimension(DimensionUnit.Centimeter, value);
                case "mm": return new Dimension(DimensionUnit.Millimeter, value);
                case "in": return new Dimension(DimensionUnit.Inch, value);
                case "em": return new Dimension(DimensionUnit.FontSize, value);
                case "%": return new Dimension(DimensionUnit.ContainerWidth, value);
                default: throw new ArgumentException("Invalid dimension");
            }
        }

        public static Dimension operator +(Dimension a, Dimension b)
        {
            if (b.IsEmpty) return a;
            if (a.IsEmpty) return b;
            return new Dimension(a.content.Concat(b.content).ToList());
        }

        public static Dimension operator -(Dimension a, Dimension b)
        {
            if (b.IsEmpty) return a;
            var res = new List<(DimensionUnit Unit, double Value)>();
            res = res.Concat(a.content).ToList();
            foreach (var i in b.content)
            {
                res.Add((i.Unit, -i.Value));
            }
            return new Dimension(res);
        }

        public bool IsEmptyOrZero(double fontSize, double width)
        {
            if (IsEmpty) return true;

            return Eval(fontSize, width) == 0.0;
        }
    }
}