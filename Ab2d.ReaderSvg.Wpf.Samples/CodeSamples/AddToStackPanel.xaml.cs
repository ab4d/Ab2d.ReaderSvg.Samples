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

namespace Ab2d.Samples.ReaderSvgSamples.CodeSamples
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class AddToStackPanel : UserControl
    {
        DragAndDropHelper _dragAndDropHelper;

        public AddToStackPanel()
        {
            InitializeComponent();

            _dragAndDropHelper = new DragAndDropHelper(this, ".svg;.gsvg");
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
            UIElement importedElement;

            importedElement = Ab2d.ReaderSvg.Instance.Read(fileName);

            MainStackPanel.Children.Add(importedElement);
        }
    }
}