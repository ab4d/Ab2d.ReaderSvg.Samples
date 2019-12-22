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


namespace Ab2d.Samples.ReaderSvgSamples.ProgressReport
{
    /// <summary>
    /// Interaction logic for ProgressReportSample.xaml
    /// </summary>

    public partial class ProgressReportSample : UserControl
    {
        private string _currentFileName;
        DragAndDropHelper _dragAndDropHelper;

        public ProgressReportSample()
        {
            InitializeComponent();

            _dragAndDropHelper = new DragAndDropHelper(this, ".svg;.svgz");
            _dragAndDropHelper.FileDroped += new EventHandler<FileDropedEventArgs>(dragAndDropHelper_FileDroped);
        }

        void dragAndDropHelper_FileDroped(object sender, FileDropedEventArgs e)
        {
            LoadSvg(e.FileName);
        }

        private void ShowOpenFileDialog()
        {
            string fileName = OpenSvgFileHelper.ShowOpenFileDialog();
            if (fileName != null)
                LoadSvg(fileName);
        }

        private void LoadSvg(string fileName)
        {
            UIElement svgElement;
            Ab2d.ReaderSvg myReaderSvg;

            DragDropMessage.Visibility = Visibility.Collapsed;

            _currentFileName = fileName;

            try
            {
                myReaderSvg = new Ab2d.ReaderSvg();
                myReaderSvg.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(myReaderSvg_ProgressChanged);

                ProgressController.Instance.ShowProgressWindow(Convert.ToInt32(InitialDelaySlider.Value));
                ProgressController.Instance.SetStatus("Reading svg file...");

                svgElement = myReaderSvg.Read(_currentFileName);

                ProgressController.Instance.SetStatus("Reading complete. Showing WPF objects...");

                MainGrid.Children.Clear();
                MainGrid.Children.Add(svgElement);
            }
            finally
            {
                Mouse.OverrideCursor = null;
                ProgressController.Instance.CloseProgressWindow();
            }
        }

        void myReaderSvg_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (LoadingDelaySlider.Value > 0)
                System.Threading.Thread.Sleep(Convert.ToInt32(LoadingDelaySlider.Value));

            ProgressController.Instance.ReportProgress(e.ProgressPercentage);
        }

        public void LoadOnClick(object sender, RoutedEventArgs e)
        {
            ShowOpenFileDialog();
        }
    }
}