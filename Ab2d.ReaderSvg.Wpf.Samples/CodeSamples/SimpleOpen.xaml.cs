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


namespace Ab2d.Samples.ReaderSvgSamples.CodeSamples
{
    /// <summary>
    /// Interaction logic for SimpleOpen.xaml
    /// </summary>

    public partial class SimpleOpen : UserControl
    {
        private string _currentFileName;
        DragAndDropHelper _dragAndDropHelper;

        public SimpleOpen()
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

            myReaderSvg = new Ab2d.ReaderSvg();

            svgElement = myReaderSvg.Read(_currentFileName);

            MainGrid.Children.Clear();
            MainGrid.Children.Add(svgElement);
        }

        public void LoadOnClick(object sender, RoutedEventArgs e)
        {
            ShowOpenFileDialog();
        }
    }
}