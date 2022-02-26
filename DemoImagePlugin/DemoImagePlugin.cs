using Orionsoft.MarkdownToPdfLib.Converters;
using Orionsoft.MarkdownToPdfLib.Plugins;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using WpfMath;

namespace DemoImagePlugin
{
    public class DemoImagePlugin : IImagePlugin
    {
        private string data;
        private IElementConverter converter;

        public ImagePluginResult Convert(string data, IElementConverter converter)
        {
            this.data = data;
            this.converter = converter;

            if (converter.Attributes?.Info == "math") return Math();
            return SomeImagePlugin();
        }

        private static ImagePluginResult SomeImagePlugin()
        {
#if DEBUG
            var fileName = Guid.NewGuid().ToString() + ".png";
#else
            var fileName = System.IO.Path.GetTempPath() +   Guid.NewGuid().ToString() + ".png";
#endif

            var w = 100;
            var h = 100;
            using (var bitmap = new Bitmap(w, h))
            {
                bitmap.SetResolution(600, 600);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.DrawLine(Pens.Black, 0, 0, w - 1, h - 1);
                    g.DrawLine(Pens.Black, w - 1, 0, 0, h - 1);
                }

                bitmap.Save(fileName, ImageFormat.Png);
            }
            return new ImagePluginResult { FileName = fileName, Success = true };
        }

        private ImagePluginResult Math()
        {
#if DEBUG
            var fileName = Guid.NewGuid().ToString() + ".png";
#else
            var fileName = System.IO.Path.GetTempPath() +   Guid.NewGuid().ToString() + ".png";
#endif

            var parser = new TexFormulaParser();
            var formula = parser.Parse(data);
            var scale = converter.Parent.FontSize;
            var renderer = formula.GetRenderer(TexStyle.Display, scale, "Arial");
            var bitmapSource = renderer.RenderToBitmap(0.0, 0.0, 600);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            using (var target = new FileStream(fileName, FileMode.Create))
            {
                encoder.Save(target);
            }
            return new ImagePluginResult { FileName = fileName, Success = true };
        }
    }
}