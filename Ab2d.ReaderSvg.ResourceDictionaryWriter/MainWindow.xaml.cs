using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Ab2d.Common.ReaderSvg;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Microsoft.Win32;

namespace Ab2d.ResourceDictionaryWriter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Ab2d.Utility.ReaderSvg.ResourceDictionaryWriter _resourceDictionaryWriter;
        private DragAndDropHelper _dragAndDropHelper;
        private Dictionary<string, FrameworkElement> _dictionaryObjects;

        private string _lastFileName;

        public MainWindow()
        {
            InitializeComponent();

            FillResourcesLimitComboBox();
            FillRootElementComboBox(importAsGeometry: true);

            // Helper class for handling drag and drop
            _dragAndDropHelper = new DragAndDropHelper(this, ".svg;.svgz");
            _dragAndDropHelper.FileDroped += _dragAndDropHelper_FileDroped;
        }

        void _dragAndDropHelper_FileDroped(object sender, FileDropedEventArgs e)
        {
            bool addAsGeometry = GeometryRadioButton.IsChecked ?? false;
            AddFile(e.FileName, addAsGeometry);
        }

        private void ImportSvgButton_OnClick(object sender, RoutedEventArgs e)
        {
            var filesToImport = GetFilesToImport();
            AddFiles(filesToImport);
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            var helpWindow = new HelpWindow();

            helpWindow.Owner = this;
            helpWindow.ShowDialog();            
        }

        private void EnsureResourceDictionaryWriter()
        {
            if (_resourceDictionaryWriter == null)
                ResetResourceDictionaryWriter();
        }

        private void ResetResourceDictionaryWriter()
        {
            _resourceDictionaryWriter = new Ab2d.Utility.ReaderSvg.ResourceDictionaryWriter();
            _resourceDictionaryWriter.XamlWriterSettings.NumberFormatString = GetSelectedNumberFormatString();

            _dictionaryObjects = new Dictionary<string, FrameworkElement>();

            XamlTextBox.Text = "";
            ResourcesListBox.Items.Clear();
        }

        private void FillResourcesLimitComboBox()
        {
            ResourcesLimitComboBox.BeginInit();
            ResourcesLimitComboBox.Items.Clear();

            AddItemToResourcesLimitComboBox("always", 1, "Extract all SolidColor Brushes and Pens from elements and define them as ResourceDictionary resources");
            AddItemToResourcesLimitComboBox("when used 2+ times", 2, "Extract SolidColor Brushes and Pens from elements and define them as ResourceDictionary resources when a Brush or Pen is used 2 times or more");
            AddItemToResourcesLimitComboBox("when used 3+ times", 3, "Extract SolidColor Brushes and Pens from elements and define them as ResourceDictionary resources when a Brush or Pen is used 3 times or more");
            AddItemToResourcesLimitComboBox("when used 5+ times", 5, "Extract SolidColor Brushes and Pens from elements and define them as ResourceDictionary resources when a Brush or Pen is used 5 times or more");
            AddItemToResourcesLimitComboBox("never", 0, "Never extract SolidColor Brushes and Pens from elements and define them as ResourceDictionary resources");

            ResourcesLimitComboBox.SelectedIndex = 0; // always
            ResourcesLimitComboBox.EndInit();
        }

        private void FillRootElementComboBox(bool importAsGeometry)
        {
            RootElementComboBox.BeginInit();
            RootElementComboBox.Items.Clear();

            if (importAsGeometry)
            {
                RootElementComboBox.Items.Add(new ComboBoxItem() {Content = "DrawingImage"});
                RootElementComboBox.Items.Add(new ComboBoxItem() {Content = "DrawingGroup"});
            }
            else
            {
                RootElementComboBox.Items.Add(new ComboBoxItem() { Content = "Viewbox" });
                RootElementComboBox.Items.Add(new ComboBoxItem() { Content = "Canvas" });
            }
            
            RootElementComboBox.EndInit();

            RootElementComboBox.SelectedIndex = 0; // always
        }

        private void AddItemToResourcesLimitComboBox(string text, int resourcesCountLimitValue, string toolTip)
        {
            var comboBoxItem = new ComboBoxItem()
            {
                Content = text,
                Tag = resourcesCountLimitValue,
                ToolTip = toolTip
            };

            ResourcesLimitComboBox.Items.Add(comboBoxItem);
        }

        private void SetResourcesCountLimit(IResourceXamlWriterSettings settings)
        {
            var comboBoxItem = (ComboBoxItem)ResourcesLimitComboBox.SelectedItem;

            settings.ResourcesCountLimit = (int) comboBoxItem.Tag;
        }

        private void AddFiles(string[] fileNames)
        {
            bool addAsGeometry = GeometryRadioButton.IsChecked ?? false;

            if (fileNames != null)
            {
                foreach (string oneFileName in fileNames)
                    AddFile(oneFileName, addAsGeometry);
            }
        }

        private void AddFile(string fileName, bool addAsGeometry)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            try
            {
                // This immediately sets the cursor to wait
                Mouse.OverrideCursor = Cursors.Wait;

                EnsureResourceDictionaryWriter();

                var keyName = GetSafeResourceName(fileName);

                if (_dictionaryObjects.ContainsKey(keyName))
                {
                    MessageBox.Show("Key " + keyName + " already added!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _lastFileName = fileName;

                // Add the file to our resource dictionary

                FrameworkElement readElement;
                if (addAsGeometry)
                {
                    if (RootElementComboBox.SelectedIndex == 0)
                        _resourceDictionaryWriter.XamlWriterSettings.RootObject = "DrawingImage";
                    else
                        _resourceDictionaryWriter.XamlWriterSettings.RootObject = "DrawingGroup";

                    readElement = _resourceDictionaryWriter.AddGeometryFile(fileName, GetGeometrySettings());
                }
                else
                {
                    if (RootElementComboBox.SelectedIndex == 0)
                        _resourceDictionaryWriter.XamlWriterSettings.RootObject = "Viewbox";
                    else
                        _resourceDictionaryWriter.XamlWriterSettings.RootObject = "Canvas";

                    // When reading as shapes we also specify the GeometrySettings because it hold the ResourcesCountLimit setting that controls if Brushes will be written as resources 
                    _resourceDictionaryWriter.GeometrySettings = GetGeometrySettings();

                    readElement = _resourceDictionaryWriter.AddFile(fileName);
                }

                if (readElement != null)
                {
                    XamlTextBox.Text = _resourceDictionaryWriter.GetXaml();

                    _dictionaryObjects.Add(keyName, readElement);


                    // Add key name and its image to left side ListBox
                    var newResourcePanel = new StackPanel();
                    newResourcePanel.Orientation = Orientation.Vertical;

                    readElement.Height = ScaleSlider.Value;

                    var nameTextBlock = new TextBlock();
                    nameTextBlock.Text = keyName + ":";

                    newResourcePanel.Children.Add(nameTextBlock);
                    newResourcePanel.Children.Add(readElement);

                    ResourcesListBox.Items.Add(newResourcePanel);
                }
            }
            finally
            {
                // set cursor back to default
                Mouse.OverrideCursor = null;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ResetResourceDictionaryWriter();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileName = GetSaveFileName();

            if (!string.IsNullOrEmpty(saveFileName))
                System.IO.File.WriteAllText(saveFileName, XamlTextBox.Text);
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(XamlTextBox.Text);
        }

        private void ScaleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_dictionaryObjects != null)
            {
                foreach (FrameworkElement oneElement in _dictionaryObjects.Values)
                    oneElement.Height = ScaleSlider.Value;
            }
        }

        private void NoDecimalsComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_resourceDictionaryWriter == null || _resourceDictionaryWriter.XamlWriterSettings == null)
                return;

            _resourceDictionaryWriter.XamlWriterSettings.NumberFormatString = GetSelectedNumberFormatString();
        }

        private string GetSelectedNumberFormatString()
        {
            if (ReferenceEquals(NoDecimalsComboBox.SelectedItem, NoDecimalsComboBox))
                return "0"; // No decimals
            else if (ReferenceEquals(NoDecimalsComboBox.SelectedItem, UnlimitedDecimalsComboBoxItem))
                return ""; // Unlimited decimals

            int noDecimals = NoDecimalsComboBox.SelectedIndex; // Number decimals is equal to selected index

            return "0." + new string('#', noDecimals); // create "0." + (noDecimals * '#')
        }

        private string ResolveResourceKeyCallback(object resource, string recommendedKey)
        {
            // This method is can be used to provide custom formating for resource keys

            // We preserve formating of all keys except root element key (get from file name)
            if (recommendedKey == _resourceDictionaryWriter.XamlWriterSettings.RootElementKeyValue) // Check if we have the root element key
                return GetSafeResourceName(_lastFileName);

            return recommendedKey;
        }

        private string GetSafeResourceName(string fileName)
        {
            var newName = System.IO.Path.GetFileNameWithoutExtension(fileName);
            newName = newName.ToLower();
            newName = newName.Replace(' ', '_');

            return newName;
        }

        private Ab2d.Common.ReaderSvg.GeometrySettings GetGeometrySettings()
        {
            Ab2d.Common.ReaderSvg.GeometrySettings settings;

            if (ReferenceEquals(OptimizationComboBox.SelectedItem, BasicOptimization))
            {
                settings = GeometrySettings.BasicOptimization;
            }
            else if (ReferenceEquals(OptimizationComboBox.SelectedItem, AdvancedOptimization))
            {
                settings = GeometrySettings.AdvancedOptimization;
            }
            else if (ReferenceEquals(OptimizationComboBox.SelectedItem, FullOptimization))
            {
                settings = GeometrySettings.FullOptimization;
            }
            else
            {
                settings = new Ab2d.Common.ReaderSvg.GeometrySettings();

                // always true
                settings.ForceCounterClockWisePointsOrder = true;
                settings.IsFrozen = true;
                settings.ConvertToStreamGeometry = true;
            }

            SetResourcesCountLimit(settings); // sets settings.ResourcesCountLimit;

            // Text is converted to PathGeometry
            // It would be also possible to convert text to GlympRun.
            // The problem with that would be that this creates text as GlympRun elements that need to have an absolute path to font file.
            // This can make the xaml problematic when usued on other computers
            // Therefore the PathGeometry is prefered option
            settings.GeometryTextExport = GeometryTextExportType.PathGeometry;

            // use custom ResolveResourceKeyCallback - set key from file name
            settings.ResolveResourceKey = ResolveResourceKeyCallback;

            return settings;
        }

        private string[] GetFilesToImport()
        {
            string [] fileNames;

            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Svg files|*.svg;*.svgz";
            openFileDialog.Multiselect = true;
            openFileDialog.Title = "Select svg files to open";
            openFileDialog.ValidateNames = true;

            var initialDir = Properties.Settings.Default.LastOpenedDirectory;
            if (string.IsNullOrEmpty(initialDir))
            {
                // try setting initial directory to sample svg files
                initialDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\Main ReaderSvg Samples\ReaderSvgSamples\Resources");
                initialDir = System.IO.Path.GetFullPath(initialDir);
            }

            // if the initial directory does not exist (any more), we will show desktop as the initial dir
            if (!System.IO.Directory.Exists(initialDir))
                initialDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            openFileDialog.InitialDirectory = initialDir;

            if ((openFileDialog.ShowDialog() ?? false) && openFileDialog.FileNames != null && openFileDialog.FileNames.Length > 0)
            {
                fileNames = openFileDialog.FileNames;

                // Seve the used directory
                Properties.Settings.Default.LastOpenedDirectory = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                Properties.Settings.Default.Save();
            }
            else
            {
                fileNames = null;
            }

            return fileNames;
        }

        private string GetSaveFileName()
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.AddExtension = true;
            saveFileDialog.CheckFileExists = false;
            saveFileDialog.CheckPathExists = true;
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.ValidateNames = false;

            saveFileDialog.FileName = "ResourceDictionary.xaml";

            saveFileDialog.DefaultExt = "xaml";
            saveFileDialog.Filter = "xaml files (*.xaml)|*.xaml";
            saveFileDialog.Title = "Select output xaml file";

            if (saveFileDialog.ShowDialog() ?? false)
                return saveFileDialog.FileName;
            
            return "";
        }

        private void ImportTypeChanged(object sender, RoutedEventArgs e)
        {
            if (!this.IsLoaded)
                return;

            Visibility visibility;
            if (GeometryRadioButton.IsChecked ?? false)
            {
                FillRootElementComboBox(importAsGeometry: true);
                ResourcesLimitTextBlock.Text = "Brushes and Pens as resources:";
                visibility = Visibility.Visible;
            }
            else
            {
                FillRootElementComboBox(importAsGeometry: false);
                ResourcesLimitTextBlock.Text = "Brushes as resources:";
                visibility = Visibility.Collapsed;
            }

            GeometryOptimizationTextBlock.Visibility = visibility;
            OptimizationComboBox.Visibility = visibility;
        }
    }
}
