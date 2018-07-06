using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
            if (args.Length < 6)
            {
                Console.WriteLine($"Usage: {Path.GetFileName(Assembly.GetEntryAssembly().Location)} <path/to/image> <image-width> <image-height> <text-size> <font> <A (Ascii)/ H (Hex) / B (Binary)>");
                Console.WriteLine($"Example: {Path.GetFileName(Assembly.GetEntryAssembly().Location)} C:\\image.jpg 1920 1080 10 Consolas B");
                return;
            }

            var location = new FileInfo(args[0]).FullName;
            var imageBitmap = new Bitmap(location);

            var imageSize = new Size(Convert.ToInt32(args[1]), Convert.ToInt32(args[2]));
            var textSize = Convert.ToInt32(args[3]);
            var gridSize = new Size(imageSize.Width / textSize, imageSize.Height / textSize);

            var font = args[4];

            var backColor = Color.Black;
            var charType = args[5];

            var newImage = new Bitmap(gridSize.Width, gridSize.Height);
            using (var g = Graphics.FromImage(newImage))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(imageBitmap, 0, 0, gridSize.Width, gridSize.Height);
            }

            var finalImage = new Bitmap(imageSize.Width, imageSize.Height);
            using (var g = Graphics.FromImage(finalImage))
            {
                g.Clear(backColor);
            }

            var charSize = new Size(textSize, textSize);
            var fontStyle = new Font(font, OptimalFontSize("X", font, charSize), FontStyle.Bold);

            for (var y = 0; y < gridSize.Height; y++)
            {
                Console.WriteLine($"Progress: {y+1} / {gridSize.Height}");
                Console.SetCursorPosition(0, Console.CursorTop - 1);

                for (var x = 0; x < gridSize.Width; x++)
                {
                    char rChar;
                    if (charType == "A")
                        rChar = GetRandomAsciiChar();
                    else if (charType == "H")
                        rChar = GetRandomHexChar();
                    else if (charType == "B")
                        rChar = GetRandomBinaryChar();
                    else
                        rChar = GetRandomAsciiChar();

                    var charImage = DrawTextImage(rChar.ToString(), fontStyle, newImage.GetPixel(x, y), backColor, charSize);

                    for (var ny = 0; ny < charSize.Height; ny++)
                    {
                        for (var nx = 0; nx < charSize.Width; nx++)
                        {
                            finalImage.SetPixel((x * charSize.Width) + nx, (y * charSize.Height) + ny, charImage.GetPixel(nx, ny));
                        }
                    }
                }
            }

            var saveLoc = Path.Combine(Path.GetDirectoryName(location), Path.GetFileNameWithoutExtension(location) + "_new.png");
            finalImage.Save(saveLoc, ImageFormat.Png);

            Console.WriteLine("File saved to " + saveLoc);
        }

        private static Bitmap DrawTextImage(String text, Font font, Color textColor, Color backColor, Size size)
        {
            var retImg = new Bitmap(size.Width, size.Height);
            using (var drawing = Graphics.FromImage(retImg))
            {
                drawing.Clear(backColor);

                using (var textBrush = new SolidBrush(textColor))
                {
                    drawing.DrawString(text, font, textBrush, 0, 0);
                    drawing.Save();
                }
            }
            return retImg;
        }

        private static float OptimalFontSize(String text, string font, Size size)
        {
            float emFloat = 1;
            float step = 1;
            while (true)
            {
                using (var img = new Bitmap(1, 1))
                {
                    using (Graphics drawing = Graphics.FromImage(img))
                    {
                        var textSize = drawing.MeasureString(text, new Font(font, emFloat));

                        if (step < 0.1)
                            break;

                        if (textSize.Height > size.Height || textSize.Width > size.Width)
                        {
                            step /= 2;
                            emFloat -= step;
                        }
                        else
                        {
                            step *= 2;
                            emFloat += step;
                        }
                    }
                }

            }
            return emFloat;
        }

        private static char GetRandomAsciiChar()
        {
            var r = new Random();
            return (char)r.Next(0x21, 0x7F);
        }

        private static char GetRandomHexChar()
        {
            var r = new Random();
            return r.Next(16).ToString("X")[0];
        }

        private static char GetRandomBinaryChar()
        {
            var r = new Random();
            return r.Next(2).ToString()[0];
        }
    }
}
