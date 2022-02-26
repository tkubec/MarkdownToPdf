// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig.Syntax;
using MigraDoc.DocumentObjectModel;
using Orionsoft.MarkdownToPdfLib.Styling;
using System.Collections.Generic;
using System.Linq;

namespace Orionsoft.MarkdownToPdfLib.Converters
{
    internal abstract class BlockConverterBase : IBlockConverter

    {
        protected List<(double Height, Color Color)> bottomMarginsPending;
        protected List<(double Height, Color Color)> topMarginsPending;

        public Block Block { get; }
        public ContainerBlockConverter Parent { get; protected set; }

        public MigraDocBlockContainer OutputContainer { get; protected set; }
        public MarkdownToPdf Owner { get; protected set; }
        public ElementAttributes Attributes { get; protected set; }
        public CascadingStyle EvaluatedStyle { get; protected set; }

        public string RawText { get; protected set; }
        public List<string> Lines { get; protected set; }

        public SingleElementDescriptor ElementDescriptor { get; protected set; }
        IBlockConverter IElementConverter.Parent { get => Parent; }
        public StylingDescriptor Descriptor => new StylingDescriptor(GetDescriptors());

        /// <summary>
        /// Calculated width of this block in points
        /// </summary>
        public double Width { get; protected set; }

        /// <summary>
        /// Calculated fontsize of this block in points
        /// </summary>
        public double FontSize { get; protected set; }

        protected BlockConverterBase(Block block, ContainerBlockConverter parent)
        {
            Block = block;
            Parent = parent;
            Owner = parent?.Owner ?? Owner;
            RawText = parent?.RawText ?? RawText;
            Lines = parent?.Lines ?? Lines;

            ElementDescriptor = new SingleElementDescriptor();
            OutputContainer = parent?.OutputContainer ?? OutputContainer;
        }

        public List<SingleElementDescriptor> GetDescriptors(List<SingleElementDescriptor> descriptors = null)
        {
            descriptors = descriptors ?? new List<SingleElementDescriptor>();

            descriptors.Add(ElementDescriptor);

            if (Parent != null && Parent.GetType() != typeof(RootBlockConvertor))
            {
                Parent.GetDescriptors(descriptors);
            }
            return descriptors;
        }

        protected CascadingStyle GetStyle()
        {
            var style = Owner?.StyleManager.GetStyle(Descriptor);
            return style;
        }

        internal void Convert()
        {
            PrepareStyling();
            Owner.OnStylingPrepared(this);

            AdjustInheritedHorizontalMargins();
            RenderSimulatedMargins(topMarginsPending, BoxSide.Top);
            if (bottomMarginsPending != null && bottomMarginsPending.Any()) EvaluatedStyle.Paragraph.KeepWithNext = true;

            if (!CreateOutput()) return;
            ApplyStyling();
            Owner.OnStylingApplied(this);
            ConvertContent();

            RenderSimulatedMargins(bottomMarginsPending, BoxSide.Bottom);
        }

        protected abstract void ConvertContent();

        protected abstract void PrepareStyling();

        protected abstract void ApplyStyling();

        protected abstract bool CreateOutput();

        #region Inherited margins

        protected virtual void PrepareVerticalInheritedMargins()
        {
            if (Parent is IStandaloneContainerConverter || Parent == null) return;

            PrepareVerticalInheritedMargin(BoxSide.Left);
            PrepareVerticalInheritedMargin(BoxSide.Right);

            // If a margin or padding is empty, MigraDoc behavior is sometimes strange, let's fix it:

            EvaluatedStyle.Padding.Left = EvaluatedStyle.Padding.Left.IsEmpty ? 0 : EvaluatedStyle.Padding.Left;
            EvaluatedStyle.Padding.Right = EvaluatedStyle.Padding.Left.IsEmpty ? 0 : EvaluatedStyle.Padding.Right;
            EvaluatedStyle.Margin.Left = EvaluatedStyle.Margin.Left.IsEmpty ? 0 : EvaluatedStyle.Margin.Left;
            EvaluatedStyle.Margin.Right = EvaluatedStyle.Margin.Left.IsEmpty ? 0 : EvaluatedStyle.Margin.Right;
        }

        protected virtual void AdjustInheritedHorizontalMargins()
        {
        }

        private void PrepareVerticalInheritedMargin(BoxSide side)
        {
            if (!Parent.EvaluatedStyle.Background.IsEmpty || !Parent.EvaluatedStyle.Padding[side].IsEmpty)
            {
                EvaluatedStyle.Padding[side] += Parent.EvaluatedStyle.Padding[side];
                EvaluatedStyle.Padding[side] += EvaluatedStyle.Margin[side];
                EvaluatedStyle.Margin[side] = Parent.EvaluatedStyle.Margin[side];
            }
            else
            {
                EvaluatedStyle.Margin[side] += Parent.EvaluatedStyle.Margin[side];
            }
        }

        protected virtual List<(double Height, Color Color)> ApplyHorizontalInheritedMargin(BoxSide side)
        {
            var horizontalMarginsPending = new List<(double Height, Color Color)>();

            if (IsMarginalBlock(Block, side))
            {
                if (Parent.EvaluatedStyle.Background.IsEmpty)
                {
                    EvaluatedStyle.Margin[side] += Parent.EvaluatedStyle.Padding[side] + Parent.EvaluatedStyle.Margin[side];
                }
                else
                {
                    horizontalMarginsPending = GetInheritedHorizontalMargins(side);
                    EvaluatedStyle.Margin[side] = 0;
                    horizontalMarginsPending = MergeHorizontalMargins(horizontalMarginsPending);

                    // now we try to merge as mutch withou adding extra paragraphs
                    if (horizontalMarginsPending.Any() && EvaluatedStyle.Border[side].Width.IsEmptyOrZero(FontSize, Width) && EvaluatedStyle.Background == horizontalMarginsPending[0].Color)
                    {
                        EvaluatedStyle.Padding[side] += horizontalMarginsPending[0].Height;
                        horizontalMarginsPending = horizontalMarginsPending.Skip(1).ToList();
                    }

                    if (horizontalMarginsPending.Any() && horizontalMarginsPending[0].Color.IsEmpty)
                    {
                        EvaluatedStyle.Margin[side] += horizontalMarginsPending[0].Height;
                        horizontalMarginsPending = horizontalMarginsPending.Skip(1).ToList();
                    }

                    if (side == BoxSide.Top) horizontalMarginsPending.Reverse();
                }
            }
            else
            {
                horizontalMarginsPending = ApplyInheritedMiddleMargin(side);
            }

            return horizontalMarginsPending;
        }

        private List<(double Height, Color Color)> ApplyInheritedMiddleMargin(BoxSide side)
        {
            var horizontalMarginsPending = new List<(double Height, Color Color)>();

            // top in the middle of container
            if (!Parent.EvaluatedStyle.Background.IsEmpty)
            {
                if (Parent.EvaluatedStyle.Background == EvaluatedStyle.Background && EvaluatedStyle.Border[side].Width.IsEmptyOrZero(FontSize, Width))

                {
                    EvaluatedStyle.Padding[side] += EvaluatedStyle.Margin[side];
                    EvaluatedStyle.Margin[side] = 0;
                }
                else
                {
                    horizontalMarginsPending.Add((EvaluatedStyle.Margin[side].Eval(FontSize, Width), Parent.EvaluatedStyle.Background));
                    EvaluatedStyle.Margin[side] = 0;
                }
            }
            return horizontalMarginsPending;
        }

        private List<(double Height, Color Color)> GetInheritedHorizontalMargins(BoxSide side)
        {
            var parent = Parent;
            var horizontalMarginsPending = new List<(double Height, Color Color)>();

            if (!EvaluatedStyle.Margin[side].IsEmptyOrZero(FontSize, Width))
            {
                horizontalMarginsPending.Add((EvaluatedStyle.Margin[side].Eval(FontSize, Width), Parent.EvaluatedStyle.Background));
            }

            while (!(parent is IStandaloneContainerConverter) && IsMarginalBlock(parent.Block, side))
            {
                if (!parent.EvaluatedStyle.Padding[side].IsEmptyOrZero(FontSize, Width))
                    horizontalMarginsPending.Add((parent.EvaluatedStyle.Padding[side].Eval(FontSize, Width), parent.EvaluatedStyle.Background));

                if (!parent.EvaluatedStyle.Margin[side].IsEmptyOrZero(FontSize, Width))
                    horizontalMarginsPending.Add((parent.EvaluatedStyle.Margin[side].Eval(FontSize, Width), parent.Parent.EvaluatedStyle.Background));

                parent = parent.Parent;
            }

            if (!(parent is IStandaloneContainerConverter))
            {
                horizontalMarginsPending.Add((parent.EvaluatedStyle.Padding[side].Eval(FontSize, Width), parent.EvaluatedStyle.Background));
                horizontalMarginsPending.Add((parent.EvaluatedStyle.Margin[side].Eval(FontSize, Width), parent.Parent.EvaluatedStyle.Background));
            }
            return horizontalMarginsPending;
        }

        private List<(double Height, Color Color)> MergeHorizontalMargins(List<(double Height, Color Color)> stripes)
        {
            var res = new List<(double Height, Color Color)>();
            if (!stripes.Any()) return res;

            (double Height, Color Color) tmp = default;
            foreach (var s in stripes)
            {
                if (tmp == default) tmp = s;
                else if (tmp.Color == s.Color)
                {
                    tmp = (tmp.Height + s.Height, tmp.Color);
                }
                else
                {
                    res.Add(tmp);
                    tmp = s;
                }
            }
            res.Add(tmp);
            return res;
        }

        private bool IsMarginalBlock(Block block, BoxSide side)
        {
            return side == BoxSide.Top ? block.IsFirst() : block.IsLast();
        }

        private void RenderSimulatedMargins(List<(double Height, Color Color)> margins, BoxSide side)
        {
            var spaceBefore = 0.0;
            MigrDocInlineContainer par = null;
            if (margins != null)
            {
                if (margins.Count >= 2 && margins[0].Color.IsEmpty)
                {
                    spaceBefore = margins[0].Height;
                }

                foreach (var s in margins)
                {
                    if (par != null && s.Color.IsEmpty)
                    {
                        par.Format.SpaceAfter += s.Height;
                        continue;
                    }
                    par = OutputContainer.AddParagraph();
                    if (spaceBefore != 0)
                    {
                        par.Format.SpaceBefore = spaceBefore;
                        spaceBefore = 0;
                    }
                    par.Format.Shading.Color = s.Color;
                    par.Format.LineSpacingRule = LineSpacingRule.Exactly;
                    par.Format.LineSpacing = s.Height;
                    par.Format.LeftIndent = (EvaluatedStyle.Margin.Left + EvaluatedStyle.Padding.Left).Eval(FontSize, Width);
                    par.Format.Borders.DistanceFromLeft = EvaluatedStyle.Padding.Left.Eval(FontSize, Width);
                    par.Format.RightIndent = (EvaluatedStyle.Margin.Right + EvaluatedStyle.Padding.Right).Eval(FontSize, Width);
                    par.Format.Borders.DistanceFromRight = EvaluatedStyle.Padding.Right.Eval(FontSize, Width);

                    if (!(side == BoxSide.Bottom && s == margins.Last()))
                    {
                        par.Format.KeepWithNext = true;
                    }
                }
            }
        }

        #endregion Inherited margins
    }
}