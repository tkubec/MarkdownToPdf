using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownToPdf
{
    public class MarkdownBulletStyle
    {
        public string Bullet { get;  set; }
        public string UncheckedBullet { get; set; }
        public string CheckedBullet { get; set; }
        

        public Font Font { get; set; }

        public Unit BulletIndent { get; set; }

        public Unit TextIndent { get; set; }

        public MarkdownBulletStyle()
        {
            Bullet = "\x2022";
            UncheckedBullet = "\x25CF";
            CheckedBullet = "\x25CC";
            Font = new Font();
            TextIndent = 14;
            BulletIndent = 10;
        }
        public MarkdownBulletStyle(MarkdownBulletStyle original)
        {

            Bullet = original.Bullet;
            Font = original.Font;
            BulletIndent = original.BulletIndent;
            TextIndent = original.TextIndent;
        }

    }
}
