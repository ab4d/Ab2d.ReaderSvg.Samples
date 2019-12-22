using System;
using System.Collections.Generic;
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
    /// Interaction logic for TrueTransformSample.xaml
    /// </summary>
    public partial class TrueTransformSample : UserControl
    {
        DragAndDropHelper _dragAndDropHelper;

        private UIElement _shownSvgElement;

        private Ab2d.ReaderSvg _readerSvg;

        private string _fileName;
        private double _canvasWidth;

        public double _canvasHeight { get; set; }


        public TrueTransformSample()
        {
            InitializeComponent();

            _dragAndDropHelper = new DragAndDropHelper(RootGrid, ".svg;.gsvg");
            _dragAndDropHelper.FileDroped += new EventHandler<FileDropedEventArgs>(dragAndDropHelper_FileDroped);

            this.Loaded += TrueTransformSample_Loaded;
        }

        void TrueTransformSample_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSvg(AppDomain.CurrentDomain.BaseDirectory + @"Resources\birthday_cake.svg");
        }

        void dragAndDropHelper_FileDroped(object sender, FileDropedEventArgs e)
        {
            LoadSvg(e.FileName);
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            ShowOpenFileDialog();
        }

        private void ApplyCustomSizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            double xValue;
            double yValue;
            double newWidth, newHeight;
            bool isValidNumber;
            ScaleTransform scale = null;
            TranslateTransform translate;
            Transform transformToApply;

            isValidNumber = ParseCustomContentSize(resizeCanvasWidthText.Text, resizeCanvasHeightText.Text, out newWidth, out newHeight);

            if (CustomWidthRadioButton.IsChecked ?? false && isValidNumber)
            {
                // If entered numbers are valid and there are new values entered
                if (isValidNumber && (Math.Abs(newWidth - _canvasWidth) > 0.1))
                    scale = new ScaleTransform(newWidth / _canvasWidth, newWidth / _canvasWidth);
            }
            else if (CustomHeightRadioButton.IsChecked ?? false && isValidNumber)
            {
                // If entered numbers are valid and there are new values entered
                if (isValidNumber && (Math.Abs(newHeight - _canvasHeight) > 0.1))
                    scale = new ScaleTransform(newHeight / _canvasHeight, newHeight / _canvasHeight);
            }

            if (scale == null)
                return;

            transformToApply = scale;

            isValidNumber = ParseCustomContentSize(OffsetXTextBox.Text, OffsetYTextBox.Text, out xValue, out yValue);

            if (isValidNumber && (xValue != 0 || yValue != 0))
                translate = new TranslateTransform(xValue, yValue);
            else
                translate = null;


            if (scale != null && translate != null)
            {
                TransformGroup group;

                group = new TransformGroup();
                group.Children.Add(translate);
                group.Children.Add(scale);

                transformToApply = group;
            }
            else if (scale != null)
            {
                transformToApply = scale;
            }
            else if (translate != null)
            {
                transformToApply = translate;
            }
            else
            {
                transformToApply = null;
            }


            if (transformToApply != null)
            {
                var transformedViewbox = _readerSvg.Transform(transformToApply, true); // true: updateLastReadViewbox - this will allow us to get transformed XAML
             
                transformedViewbox.Stretch = Stretch.None;

                UpdateCanvasSize(transformedViewbox);

                ShowSvgElement(transformedViewbox);

                // Reset the translate
                OffsetXTextBox.Text = "0.0";
                OffsetYTextBox.Text = "0.0";
            }           
        }

        private void ApplyDpiChangeSizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            int dpi;
            if (!Int32.TryParse(DpiTextBox.Text, out dpi))
                dpi = 90;

            double scale = 25.4 / (double)dpi; // 25.4 mm in one inch

            var scaleTransform = new ScaleTransform(scale, scale);

            // Transform method transforms all the coordinates, sizes and other values in the last read Viewbox with using transformation.
            // It is also possible to transform some other objetcs that are not created with ReaderSvg.
            // read more about that with checking Help file for Ab2d.Utility.ReaderSvg.TrueTransform class (this class is also used inside Transform method).
            var transformedViewbox = _readerSvg.Transform(scaleTransform, true); // true: updateLastReadViewbox - this will allow us to get transformed XAML
            transformedViewbox.Stretch = Stretch.None;

            UpdateCanvasSize(transformedViewbox);

            ShowSvgElement(transformedViewbox);
        }

        private void LoadSvg(string fileName)
        {
            if (_readerSvg == null)
                _readerSvg = new ReaderSvg();

            _fileName = fileName;
            Viewbox importedElement = _readerSvg.Read(fileName);
            importedElement.Stretch = Stretch.None;

            UpdateCanvasSize(importedElement);

            ShowSvgElement(importedElement);
        }

        private void UpdateCanvasSize(Viewbox importedElement)
        {
            if (importedElement != null && importedElement.Child is Canvas)
            {
                _canvasWidth = ((Canvas)importedElement.Child).Width;
                _canvasHeight = ((Canvas)importedElement.Child).Height;

                OriginalWidthTextBlock.Text = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.#}", _canvasWidth);
                OriginalHeightTextBlock.Text = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.#}", _canvasHeight);
            }
        }

        private void ShowSvgElement(UIElement svgElement)
        {
            if (_shownSvgElement != null)
                RootGrid.Children.Remove(_shownSvgElement);


            Grid.SetColumn(svgElement, 0);
            Grid.SetRow(svgElement, 0);
            RootGrid.Children.Add(svgElement);

            _shownSvgElement = svgElement;


            UpdateXaml();
        }

        private void UpdateXaml()
        {
            XamlTextBox.Text = _readerSvg.GetXaml();
        }

        private bool ParseCustomContentSize(string widthText, string HeightText, out double width, out double height)
        {
            bool isValidSize;

            isValidSize = true;

            if (!double.TryParse(widthText, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out width))
                isValidSize = false;

            if (!double.TryParse(HeightText, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out height))
                isValidSize = false;

            return isValidSize;
        }

        private void ShowOpenFileDialog()
        {
            string fileName = OpenSvgFileHelper.ShowOpenFileDialog();
            if (fileName != null)
                LoadSvg(fileName);
        }
    }
}
