using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace AsciiGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine($"Usage: {Path.GetFileName(Assembly.GetEntryAssembly().Location)} <path/to/image> <width> <height> <body-style (optional)>");
                Console.WriteLine($"Example: {Path.GetFileName(Assembly.GetEntryAssembly().Location)} C:\\image.jpg 100 50 background-color:#000000;font-family:monospace;font-weight:bold;");
                return;
            }

            var width = Convert.ToInt32(args[1]);
            var height = Convert.ToInt32(args[2]);

            var location = new FileInfo(args[0]).FullName;
            var imageBitmap = new Bitmap(location);

            var xInterval = (double)imageBitmap.Width / (double)width;
            var yInterval = (double)imageBitmap.Height / (double)height;

            var charArray = new ColoredChar[width, height];

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var rArr = new List<int>();
                    var gArr = new List<int>();
                    var bArr = new List<int>();

                    for (var i = 0; i < (int)xInterval; i++)
                    {
                        for (var j = 0; j < (int)yInterval; j++)
                        {
                            var curX = (int)(x * xInterval) + i;
                            var curY = (int)(y * yInterval) + j;
                            var pixelColor = imageBitmap.GetPixel(curX, curY);
                            rArr.Add(pixelColor.R);
                            gArr.Add(pixelColor.G);
                            bArr.Add(pixelColor.B);
                        }
                    }

                    charArray[x, y] = new ColoredChar(Convert.ToByte(rArr.Average()), Convert.ToByte(gArr.Average()), Convert.ToByte(bArr.Average()));
                }
            }

            var style = "background-color:#000000;font-family:monospace;font-weight:bold;";
            if (args.Length > 3)
                style = args[3];

            var sb = new StringBuilder($"<html><body style=\"{style}\">");

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    sb.Append("<font color=\"#");
                    sb.Append(charArray[x, y].R.ToString("X2"));
                    sb.Append(charArray[x, y].G.ToString("X2"));
                    sb.Append(charArray[x, y].B.ToString("X2"));
                    sb.Append("\">");
                    sb.Append(HttpUtility.HtmlEncode(charArray[x, y].Character));
                    sb.Append("</font>");
                }
                sb.Append("<br />");
            }

            sb.Append("</body></html>");

            var htmlLoc = Path.Combine(Path.GetDirectoryName(location), Path.GetFileNameWithoutExtension(location) + ".html");
            File.WriteAllText(htmlLoc, sb.ToString());

            Console.WriteLine("File saved to " + htmlLoc);
        }

        public class ColoredChar
        {
            public char Character { get; set; }
            public byte R { get; set; }
            public byte G { get; set; }
            public byte B { get; set; }

            public ColoredChar()
            {
                var r = new Random();
                Character = (char)r.Next(0x21, 0x7F);
            }

            public ColoredChar(byte r, byte g, byte b) : this()
            {
                R = r;
                G = g;
                B = b;
            }
        }
    }
}
