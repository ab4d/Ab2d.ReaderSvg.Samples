using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ab2d.Samples.ReaderSvgSamples.Other
{
    /// <summary>
    /// Interaction logic for RenderToBitmapSample.xaml
    /// </summary>
    public partial class RenderToBitmapSample : UserControl
    {
        private string _svgFileName;
        private ReaderSvg _readerSvg;

        public RenderToBitmapSample()
        {
            InitializeComponent();

            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            string startupFileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\paint.svg");
            LoadSvgFile(startupFileName);
        }

        private void LoadSvgFile(string svgFileName)
        {
            _readerSvg = new Ab2d.ReaderSvg();
            Viewbox readViewbox = _readerSvg.Read(svgFileName);

            SvgBorder.Child = readViewbox;

            _svgFileName = svgFileName;
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.AddExtension = true;
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            saveFileDialog.FileName = System.IO.Path.ChangeExtension(System.IO.Path.GetFileName(_svgFileName), ".png");

            saveFileDialog.DefaultExt = "png";
            saveFileDialog.Filter = "PNG (*.png)|*.png";
            saveFileDialog.Title = "Select output png file";

            if (saveFileDialog.ShowDialog() ?? false)
            {
                // render read svg image to bitmap
                var selectedSize = GetSelectedSize();

                BitmapSource renderToBitmap;

                if (selectedSize.IsEmpty)
                    renderToBitmap = _readerSvg.RenderToBitmap(backgroundBrush: null); // No custom size
                else
                    renderToBitmap = _readerSvg.RenderToBitmap(backgroundBrush: null, customWidth: (int)selectedSize.Width, customHeight: (int)selectedSize.Height); // No custom size

                // NOTE:
                // It is also possible to specify custom DPI settings when exporting to bitmap

                // Save bitmap
                using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    PngBitmapEncoder enc = new PngBitmapEncoder();
                    BitmapFrame bitmapImage = BitmapFrame.Create(renderToBitmap);
                    enc.Frames.Add(bitmapImage);
                    enc.Save(fs);
                }

                // Show exported image in image viewer
                System.Diagnostics.Process.Start(saveFileDialog.FileName); 
            }
        }

        private Size GetSelectedSize()
        {
            var comboBoxItem = SizeComboBox.SelectedItem as ComboBoxItem;
            if (comboBoxItem == null)
                return Size.Empty; // This will use the size of the svg

            var contentString = comboBoxItem.Content as string;

            string[] sizeParts = contentString.Split('x');

            int width, height;
            if (Int32.TryParse(sizeParts[0], out width) && Int32.TryParse(sizeParts[1], out height))
                return new Size(width, height);

            return Size.Empty;
        }
    }
}
