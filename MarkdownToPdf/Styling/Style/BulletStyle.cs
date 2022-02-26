// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

namespace Orionsoft.MarkdownToPdfLib.Styling
{
    /// <summary>
    /// Part of <see cref="CascadingStyle"/> defining bullets or numbering for list items, footnotes and thematic breaks
    /// </summary>
    public class BulletStyle
    {
        /// <summary>
        /// Bullet or numbering for normal list items, footnotes and thematic breaks
        /// </summary>
        public SingleBulletStyle Normal { get; set; }

        /// <summary>
        /// Unchecked bullet style in tasklists
        /// </summary>
        public SingleBulletStyle Unchecked { get; set; }

        /// <summary>
        /// Checked bullet style in tasklists
        /// </summary>
        public SingleBulletStyle Checked { get; set; }

        /// <summary>
        /// Indentation of the bullet's right side (if the bullet or numbering is longer, it expands to the left)
        /// </summary>
        public Dimension BulletIndent { get; set; }

        /// <summary>
        /// Indentation of the text following the bullet, relates to the begining of line
        /// </summary>
        public Dimension TextIndent { get; set; }

        internal BulletStyle()
        {
            Normal = new SingleBulletStyle();
            Unchecked = new SingleBulletStyle();
            Checked = new SingleBulletStyle();
            TextIndent = new Dimension();
            BulletIndent = new Dimension();
        }

        internal BulletStyle MergeWith(BulletStyle baseStyle)
        {
            var res = new BulletStyle
            {
                TextIndent = !TextIndent.IsEmpty ? TextIndent : baseStyle.TextIndent,
                BulletIndent = !BulletIndent.IsEmpty ? BulletIndent : baseStyle.BulletIndent,
                Normal = Normal.MergeWith(baseStyle.Normal),
                Checked = Checked.MergeWith(baseStyle.Checked),
                Unchecked = Unchecked.MergeWith(baseStyle.Unchecked)
            };
            return res;
        }
    }
}