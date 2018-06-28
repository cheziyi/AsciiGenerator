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
            if (args.Length != 4)
            {
                Console.WriteLine($"Usage: {Path.GetFileName(Assembly.GetEntryAssembly().Location)} <path/to/image> <width> <height> <font>");
                return;
            }

            var width = Convert.ToInt32(args[1]);
            var height = Convert.ToInt32(args[2]);

            var location = new FileInfo(args[0]).FullName;
            var imageBitmap = new Bitmap(location);

            var xInterval = imageBitmap.Width / width;
            var yInterval = imageBitmap.Height / height;

            var charArray = new ColoredChar[width, height];

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var rArr = new List<int>();
                    var gArr = new List<int>();
                    var bArr = new List<int>();

                    for (var i = 0; i < xInterval; i++)
                    {
                        for (var j = 0; j < yInterval; j++)
                        {
                            var curX = x * xInterval + i;
                            var curY = y * yInterval + j;
                            var pixelColor = imageBitmap.GetPixel(curX, curY);
                            rArr.Add(pixelColor.R);
                            gArr.Add(pixelColor.G);
                            bArr.Add(pixelColor.B);
                        }
                    }

                    charArray[x, y] = new ColoredChar(Convert.ToByte(rArr.Average()), Convert.ToByte(gArr.Average()), Convert.ToByte(bArr.Average()));
                }
            }

            var sb = new StringBuilder($"<html><body><font face=\"{args[3]}\">");

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

            sb.Append("</font></body></html>");

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
