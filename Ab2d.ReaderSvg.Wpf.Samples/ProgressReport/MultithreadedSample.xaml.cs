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
using System.Windows.Shapes;
using System.Net;
using System.IO;
using System.Xml;
using System.Windows.Markup;
using System.ComponentModel;
using System.Threading;
using System.Windows.Media.Animation;


namespace Ab2d.Samples.ReaderSvgSamples.ProgressReport
{
    /// <summary>
    /// Interaction logic for MultithreadedSample.xaml
    /// </summary>

    public partial class MultithreadedSample : UserControl
    {
        DragAndDropHelper _dragAndDropHelper;
        private Exception _readException;
        private DrawingImage _svgDrawingImage;
        private int _loadDelay;
        private bool _isAsyncLoad;


        public MultithreadedSample()
        {
            InitializeComponent();

            _dragAndDropHelper = new DragAndDropHelper(this, ".svg;.svgz");
            _dragAndDropHelper.FileDroped += new EventHandler<FileDropedEventArgs>(dragAndDropHelper_FileDroped);
        }

        void dragAndDropHelper_FileDroped(object sender, FileDropedEventArgs e)
        {
            LoadSvg(e.FileName);
        }

        public void LoadOnClick(object sender, RoutedEventArgs e)
        {
            ShowOpenFileDialog();
        }

        private void ShowOpenFileDialog()
        {
            string fileName = OpenSvgFileHelper.ShowOpenFileDialog();
            if (fileName != null)
                LoadSvg(fileName);
        }

        private void LoadSvg(string fileName)
        {
            SvgGrid.Children.Clear();
            DragDropMessage.Visibility = Visibility.Collapsed;
            ProgressGrid.Visibility = Visibility.Visible;
            

            DoubleAnimation rotateAnimation;
            rotateAnimation = new DoubleAnimation();
            rotateAnimation.From = 0;
            rotateAnimation.To = 360;
            rotateAnimation.Duration = new Duration(TimeSpan.FromSeconds(2));
            rotateAnimation.RepeatBehavior = RepeatBehavior.Forever;

            RectangleRotate.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);

            _loadDelay = Convert.ToInt32(LoadingDelaySlider.Value);
            _isAsyncLoad = LoadAsyncCheckBox.IsChecked ?? false;

            if (_isAsyncLoad)
            {
                // Create in new STA thread
                Thread newWindowThread = new Thread(new ParameterizedThreadStart(ReadSvgFile));
                newWindowThread.SetApartmentState(ApartmentState.STA); // WPF only runs on STA
                newWindowThread.IsBackground = true;
                newWindowThread.Start(fileName);
            }
            else
            {
                // load on the same thread
                ReadSvgFile(fileName);
            }
        }

        private void ReadComplete()
        {
            Image newSvgImage;

            ProgressGrid.Visibility = Visibility.Collapsed;

            if (_svgDrawingImage != null)
            {
                newSvgImage = new Image();
                newSvgImage.Source = _svgDrawingImage;

                SvgGrid.Children.Clear();
                SvgGrid.Children.Add(newSvgImage);
            }
            else
            {
                if (_readException != null)
                    MessageBox.Show("Exception when reading svg file:\r\n" + _readException.Message);
            }
        }

        private void ReportProgress(int progressPercentage)
        {
            ProgressBar1.Value = progressPercentage;
        }

        private void ReadSvgFile(object param)
        {
            string fileName;
            Ab2d.ReaderSvg myReaderSvg;
            
            Image readImage;

            fileName = param as string;

            try
            {
                _readException = null;
                _svgDrawingImage = null;

                myReaderSvg = new Ab2d.ReaderSvg();

                myReaderSvg.ProgressChanged += new ProgressChangedEventHandler(myReaderSvg_ProgressChanged);

                // NOTE:
                // Shape (Canvas, Path, Rectangle, ect.) objects cannot be frozen.
                // Therefore we need to call ReadGeometry to get DrawingImage instead of Read method
                readImage = myReaderSvg.ReadGeometry(fileName);

                _svgDrawingImage = readImage.Source as DrawingImage;

                // If we were loading the svg file on background thread we need to Freeze the DrawingImage
                // Without freezing the read DrawingImage would be only accessable on this thread (and not on the main UI thread)
                // NOTE: The Image is not derived from Freezable and therefore cannot be frozen
                if (_isAsyncLoad && _svgDrawingImage != null)
                {
                    if (_svgDrawingImage.CanFreeze)
                    {
                        _svgDrawingImage.Freeze();
                    }
                    else
                    {
                        _svgDrawingImage = null;
                        _readException = new Exception("Cannot freeze the read DrawingImage");
                    }
                }
            }
            catch (Exception ex)
            {
                _readException = ex;
            }
            finally
            {
                if (_isAsyncLoad)
                {
                    // run the method on the MainWindow's (this) thread
                    this.Dispatcher.Invoke(new System.Threading.ThreadStart(() => ReadComplete()));
                }
                else
                {
                    // run on the same thread
                    ReadComplete();
                }
            }
        }

        void myReaderSvg_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            System.Threading.Thread.Sleep(_loadDelay);

            if (_isAsyncLoad)
            {
                // run the method on the MainWindow's (this) thread
                this.Dispatcher.Invoke(new System.Threading.ThreadStart(() => ReportProgress(e.ProgressPercentage)));
            }
            else
            {
                // run on the same thread
                ReportProgress(e.ProgressPercentage);
            }
        }
    }
}