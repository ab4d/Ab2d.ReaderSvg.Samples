#define USE_ZOOMPANEL // If ZoomPanel is not installed, than remove it from references and comment this line to build the project without ZoomPanel control

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using Ab2d.Controls;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using MessageBox = System.Windows.Forms.MessageBox;

namespace ReaderSvg.WinFormsSample
{
    // This sample shows you how easy is to enjoy the vector based WPF graphics inside WinForms application.
    // You do not need to learn all the things that come with WPF - XAML, binding, DepandencyProperties, etc.
    // You only need to add the following references:
    // - PresentationCore
    // - PresentationFramework
    // - WindowsBase
    // - WindowsFormsIntegration
    // 
    // After that you can use WPF controls inside WinForms application with ElementHost control
    // This sample shows how easy is to define all the WPF controls in code behind (but you could also create a new WPF's UserControl and define everything in XAML and then in WinForms designer assign the UserControl to the ElementHost control
    // The sample is also showing how easy is to read a svg file with ReaderSvg and use ZoomPanel control to allow users to zoom and pan the svg image.

    public partial class Form1 : Form
    {
        private string[] _svgSamples;
        private int _currentlyShownSampleIndex;

        public Form1()
        {
            InitializeComponent();

#if USE_ZOOMPANEL
            InitializeZoomPanelControl();
#endif

            this.Load += (sender, args) => ShowNextSample(); // Show first svg sample
        }

#if USE_ZOOMPANEL
        private Ab2d.Controls.ZoomPanel _zoomPanel;
        private Ab2d.Controls.ZoomController _zoomController;

        // This method adds ZoomPanel control to the WPF ElementHost control
        // It will be used to add zooming and panning to the read svg file - to allow users to explore the all the details of the vector based svg image
        // To see more samples of the ZoomPanel, check the samples that come with the control. You can easily port the samples to WinForms.
        private void InitializeZoomPanelControl()
        {
            _zoomPanel = new ZoomPanel()
            {
                ZoomMode = ZoomPanel.ZoomModeType.Move,
                Stretch = Stretch.Uniform,
                IsMouseWheelZoomEnabled = true,
                IsZoomPositionPreserved = true,
                IsAnimated = true
            };

            _zoomController = new ZoomController()
            {
                TargetZoomPanel = _zoomPanel,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            // We create a root grid that will host ZoomPanel and ZoomController (ElementHost control can host only one child control - this will be the rootGrid)
            var rootGrid = new Grid();
            rootGrid.Children.Add(_zoomPanel);
            rootGrid.Children.Add(_zoomController);

            elementHost1.Child = rootGrid;


            // HACK HACK HACK:
            // To pass mouse wheel events to WPF controls, we need to manually pass the mouse wheel event to WPF
            // The following class does that for us:
            MouseWheelMessageFilter.RegisterMouseWheelHandling(rootGrid);
        }
#endif

        private void ShowNextSample()
        {
            int sampleIndex;

            if (_svgSamples == null)
            {
                // Get all svg samples from embeded resources
                _svgSamples = Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(f => f.EndsWith(".svg")).ToArray();
                sampleIndex = 0;
            }
            else
            {
                sampleIndex = _currentlyShownSampleIndex + 1; // Show next
                if (sampleIndex >= _svgSamples.Length)
                    sampleIndex = 0; // Show first image
            }

            // Load svg file into WPF's Viewbox
            Viewbox readSvgViewbox = LoadSvgFileFromEmbeddedResources(_svgSamples[sampleIndex]);

            if (readSvgViewbox != null)
                ShowReadViewbox(readSvgViewbox);

            _currentlyShownSampleIndex = sampleIndex;
        }

        private Viewbox LoadSvgFileFromEmbeddedResources(string svgResourceName)
        {
            Viewbox readSvgViewbox = null;

            using (var svgFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(svgResourceName))
            {
                if (svgFileStream == null)
                {
                    MessageBox.Show("Cannot open " + svgResourceName, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                readSvgViewbox = LoadSvgFile(null, svgFileStream);
            }

            return readSvgViewbox;
        }

        private Viewbox LoadSvgFile(string svgFileName, Stream svgFileStream)
        {
            // Create an instance of Ab2d.ReaderSvg
            // This way we will be able to set some properties before reading the svg file
            // and also access some additional data after the svg file is read
            var readerSvg = new Ab2d.ReaderSvg();

            // By default the read objects are automatically sized to fit the content.
            // But if you would like to preserve the size defined in svg header element (for example letter size), than set the AutoSize property to false:
            //readerSvg.AutoSize = false;
            
            Viewbox svgViewbox;

            if (svgFileName != null)
                svgViewbox = readerSvg.Read(svgFileName);
            else
                svgViewbox = readerSvg.Read(svgFileStream);


            if (scaleToFitCheckBox.Checked)
                svgViewbox.Stretch = Stretch.Uniform; // The content of Viewbox will be scaled to fit the size of the Viewbox (this is already set value - I have added it here for clarity)
            else
                svgViewbox.Stretch = Stretch.None; // Viewbox will not scale the content

            return svgViewbox;
        }

        private void ShowReadViewbox(Viewbox viewbox)
        {
            // Add some margin to read svg image
            // We need to add margin to the child control (adding margin or padding to viewbox or to elementHost does not work)
            if (viewbox.Child is FrameworkElement)
                ((FrameworkElement)viewbox.Child).Margin = new Thickness(10, 10, 10, 10); // Add some margin

            
#if USE_ZOOMPANEL
            _zoomPanel.Content = viewbox;
#else
            elementHost1.Child = viewbox;
#endif
            
            Bitmap gdiBitmap = RenderWpfObjectToGdiBitmap((Canvas)viewbox.Child);
            pictureBox1.Image = gdiBitmap;
        }

        private Bitmap RenderWpfObjectToGdiBitmap(FrameworkElement wpfObject)
        {
            // First render the wpf objects created from svg file into wpf bitmap
            // We specify custom size of the image and 4x antialiasing
            // Antialiasing improves the image quality by rendering the image 4x bigger and than scaling the bitmap down to original size
            var wpfBitmap = ImagesHelper.RenderToBitmap(wpfObject, pictureBox1.Width, pictureBox1.Height, 4, null);

            // We cannot convert wpfBitmap directly to GDI+ bitmap
            // Therefore we do that with creating the png bitmap stream
            // and pass it to GDI Bitmap constructor

            var encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
            encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(wpfBitmap));


            Bitmap gdiBitmap;

            using (var stream = new System.IO.MemoryStream())
            {
                encoder.Save(stream);
                gdiBitmap = new Bitmap(stream);
            }

            return gdiBitmap;
        }

        private void showNextButton_Click(object sender, EventArgs e)
        {
            ShowNextSample();
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog()
            {
                DefaultExt = "svg",
                Filter = "svg files (*.svg;*.svgz)|*.svg;*.svgz",
                Multiselect = false,
                Title = "Select svg or svgz file to open"
            };

            var dialogResult = openFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                var readSvgViewbox = LoadSvgFile(openFileDialog.FileName, null);

                if (readSvgViewbox != null)
                    ShowReadViewbox(readSvgViewbox);
            }
        }
    }
}
