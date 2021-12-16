using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SvgToPngConverter
{
    class Program
    {
        [STAThread] // This is required by WPF
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                WriteUsateText();
                return;
            }

            int customWidth = 0;
            int customHeight = 0;
            string outputFolder = null;
            bool autoSize = false;
            Brush backgroundBrush = null;
            int dpi = 96;

            var inputFileNames = new List<string>();

            foreach (var oneArgument in args)
            {
                if (oneArgument[0] == '/' || oneArgument[0] == '-')
                {
                    // Check the command
                    try
                    {
                        string[] commandParts = oneArgument.Substring(1).Split(':');

                        string command = commandParts[0].ToLower();

                        if (command == "w" || command == "width")
                        {
                            customWidth = Int32.Parse(commandParts[1]);
                        }
                        else if (command == "h" || command == "height")
                        {
                            customHeight = Int32.Parse(commandParts[1]);
                        }
                        else if (command == "o" || command == "out")
                        {
                            outputFolder = commandParts[1];
                        }
                        else if (command == "a" || command == "autosize")
                        {
                            autoSize = true;
                        }
                        else if (command == "b" || command == "background")
                        {
                            backgroundBrush = GetBrush(commandParts[1]);
                        }
                        else if (command == "d" || command == "dpi")
                        {
                            dpi = Int32.Parse(commandParts[1]);
                        }
                        else if (command == "h" || command == "?")
                        {
                            WriteUsateText();
                            return;
                        }
                        else
                        {
                            Console.WriteLine("Unknown argument: " + command);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error processing argument: " + oneArgument);
                    }
                }
                else
                {
                    inputFileNames.Add(oneArgument);
                }
            }

            if (inputFileNames.Count == 0)
                WriteUsateText();

            foreach (var fileName in inputFileNames)
            {
                SvgToPng(fileName, autoSize, customWidth, customHeight, dpi, backgroundBrush, outputFolder);
            }
        }

        private static Brush GetBrush(string brushText)
        {
            Brush brush;

            try
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Brush));
                brush = converter.ConvertFromInvariantString(brushText) as Brush;
            }
            catch
            {
                brush = null;
            }

            return brush;
        }

        private static void SvgToPng(string svgFileName, bool autoSize, int customWidth, int customHeight, int dpi, Brush backgroundBrush, string outputFolder)
        {
            Console.WriteLine("Start reading svg file: " + svgFileName);

            var readerSvg = new Ab2d.ReaderSvg();
            readerSvg.AutoSize = autoSize;

            try
            {
                var svgViewbox = readerSvg.Read(svgFileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading svg file: " + ex.Message);
                return;
            }

            Console.WriteLine("Svg file read. Start rendering to bitmap...");

            var wpfBitmap = readerSvg.RenderToBitmap(customWidth, customHeight, dpi, backgroundBrush);

            Console.WriteLine("Bitmap created. Start saving...");

            string outputFileName;

            if (!string.IsNullOrEmpty(outputFolder))
                outputFileName = System.IO.Path.Combine(outputFolder, System.IO.Path.GetFileName(svgFileName)); // Change folder
            else
                outputFileName = svgFileName;

            outputFileName = System.IO.Path.ChangeExtension(outputFileName, ".png"); // Change the extention to png

            var encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();

            // If you want to save to jpg use the following code:
            //var encoder = new JpegBitmapEncoder();
            //encoder.QualityLevel = 85;

            encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(wpfBitmap));

            try
            {
                using (var stream = File.OpenWrite(outputFileName))
                {
                    encoder.Save(stream);
                }

                Console.WriteLine("Saving complete");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving png image: " + ex.Message);
            }
        }

        private static void WriteUsateText()
        {
            string usageText =
                @"SvgToPngConverter by AB4D d.o.o. (sample for using Ab2d.ReaderSvg library)
Usage:
SvgToPngConverter [/width:x] [/w:x] [/height:y] [/h:y] [/autosize] [/a] [/background:color] [b:color] [/d] [/dpi] [/out:folder] [/o:folder] file1.svg file2.svg file3.svg ...

Optional parameters:
[/width:x] [/w:x]  - specify custom width of the png file. If not specified then the width defined in svg file will be used or the if the height is set, the width will be set from the height to preserve aspect ratio.
[/height:y] [/h:y] - specify custom width of the png file. If not specified then the height defined in svg file will be used or the if the width is set, the height will be set from the width to preserve aspect ratio.
[/out:folder] [/o:folder] - specify custom output folder. If not specified, then the same folder as the input image is used.
[/autosize] [/a] - when specified the objects in svg file are scaled to fit into the whole image. See ReaderSvg.AutoSize for more info.
[/background:color] [b:color] - specify background color (use same text as in XAML). If omitted than background will be transparent.
[/dpi:value] [d:value] - specify the dpi setting of the created image. If omitted than 96 is used for dpi.
[/?] [/h] - show this usages help

Example:
SvgToPngConverter /w:800 /b:White myImage.svg";

            Console.WriteLine(usageText);
            Console.ReadKey();
        }
    }
}
