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


namespace Ab2d.Samples.ReaderSvgSamples.Other
{
    /// <summary>
    /// Interaction logic for GetXamlSample.xaml
    /// </summary>

    public partial class GetXamlSample : UserControl
    {
        private DragAndDropHelper _dragAndDropHelper;
        private Ab2d.ReaderSvg _myReaderSvg;

        public GetXamlSample()
        {
            InitializeComponent();

            _myReaderSvg = new Ab2d.ReaderSvg();

            _dragAndDropHelper = new DragAndDropHelper(this, ".svg;.svgz");
            _dragAndDropHelper.FileDroped += new EventHandler<FileDropedEventArgs>(dragAndDropHelper_FileDroped);
        }

        void dragAndDropHelper_FileDroped(object sender, FileDropedEventArgs e)
        {
            LoadSvg(e.FileName);
        }

        private void OnXamlWritterSettingsChanged(object sender, RoutedEventArgs e)
        {
            if (!this.IsLoaded)
                return; // not yet initialized

            UpdateXaml();
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

            Mouse.OverrideCursor = Cursors.Wait;
            SvgGrid.Children.Clear();

            svgElement = _myReaderSvg.Read(fileName);
            
            SvgGrid.Children.Add(svgElement);

            UpdateXaml();

            Mouse.OverrideCursor = null;
        }

        private void UpdateXaml()
        {
            if (_myReaderSvg.LastReadViewbox == null) // no svg file was read yet
                return;

            if (AdvancedOptionsCheckBox.IsChecked ?? false)
            {
                Ab2d.Common.ReaderSvg.BaseXamlWriterSettings xamlWritterSettings;

                if (WpfRadioButton.IsChecked ?? false)
                    xamlWritterSettings = new Ab2d.Common.ReaderSvg.WpfXamlWriterSettings();
                else
                    xamlWritterSettings = new Ab2d.Common.ReaderSvg.SilverlightXamlWriterSettings();

                if (UnlimitedDecimalsRadioButton.IsChecked ?? false)
                    xamlWritterSettings.NumberFormatString = "";
                else if (NoDecimalsRadioButton.IsChecked ?? false)
                    xamlWritterSettings.NumberFormatString = "0";
                else // one decimal
                    xamlWritterSettings.NumberFormatString = "0.#";

                if (CanvasRadioButton.IsChecked ?? false)
                    xamlWritterSettings.RootObject = "Canvas";
                if (UserControlRadioButton.IsChecked ?? false)
                    xamlWritterSettings.RootObject = "UserControl";
                else
                    xamlWritterSettings.RootObject = "";

                xamlWritterSettings.UseColorNames = UseColorNamesCheckBox.IsChecked ?? false;

                XamlTextBox.Text = _myReaderSvg.GetXaml(xamlWritterSettings);
            }
            else
            {
                // Use default XAML Writter settings
                XamlTextBox.Text = _myReaderSvg.GetXaml();
            }
        }

        public void LoadOnClick(object sender, RoutedEventArgs e)
        {
            ShowOpenFileDialog();
        }
    }
}