// This file is a part of MarkdownToPdf Library by Tomas Kubec
// Distributed under MIT license - see license.txt
//

namespace Orionsoft.MarkdownToPdfLib.Styling
{
    /// <summary>
    /// Part of <see cref="BulletStyle"/> defining the bullet itself
    /// </summary>
    public class SingleBulletStyle
    {
        /// <summary>
        /// In case of an un-numbered item, text representing the bullet. In case of a numbered item the text being appended affter the number
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Font style of the bullet or numbering
        /// </summary>
        public FontStyle Font { get; set; }

        internal SingleBulletStyle()
        {
            Font = new FontStyle();
        }

        internal SingleBulletStyle MergeWith(SingleBulletStyle baseStyle)
        {
            var res = new SingleBulletStyle
            {
                Content = Content.HasValue() ? Content : baseStyle.Content,
                Font = Font.MergeWith(baseStyle.Font)
            };
            return res;
        }
    }
}