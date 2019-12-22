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
using System.Data;

namespace Ab2d.Samples.ReaderSvgSamples.Visio
{
    /// <summary>
    /// Interaction logic for NetworkWithEmbeddedData.xaml
    /// </summary>
    public partial class NetworkWithEmbeddedData : UserControl
    {
        private Canvas _selectedCanvas;

        public NetworkWithEmbeddedData()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(Window1_Loaded);
        }

        void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            DataTable svgData;

            svgData = SvgNetwork.InnerReaderSvg.GetCustomPropertiesDataTable();

            // Note that ItemsSource="{Binding Path=.}" must be set on DataGrid in order to set a DataTable to the DataContext
            DataGrid1.DataContext = svgData;

            RegisterMouseOverEvents(svgData);
        }

        private void RegisterMouseOverEvents(DataTable svgData)
        {
            foreach (DataRow dr in svgData.Rows)
            {
                Canvas dataCanvas;

                dataCanvas = GetCanvas((string)dr[0]); // first column contains the name of the linkes element

                dataCanvas.MouseEnter += new MouseEventHandler(dataCanvas_MouseEnter);
                dataCanvas.MouseLeave += new MouseEventHandler(dataCanvas_MouseLeave);
                dataCanvas.MouseMove += new MouseEventHandler(dataCanvas_MouseMove);
            }
        }

        void dataCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            UpdateCustomDataPosition();
        }

        void dataCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            CustomDataBorder.Visibility = Visibility.Collapsed;
        }

        void dataCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            Dictionary<string, object> customProperties;

            customProperties = SvgNetwork.InnerReaderSvg.GetCustomProperties(sender);

            CustomDataListBox.ItemsSource = customProperties;


            UpdateCustomDataPosition();
        }

        private void UpdateCustomDataPosition()
        {
            Point mousePosition;

            mousePosition = Mouse.GetPosition(this);
            mousePosition.X += 10;
            mousePosition.Y += 5;

            CustomDataBorder.RenderTransform = new TranslateTransform(mousePosition.X, mousePosition.Y);

            if (CustomDataBorder.Visibility != Visibility.Visible)
                CustomDataBorder.Visibility = Visibility.Visible;
        }

        private Canvas GetCanvas(string objectName)
        {
            return SvgNetwork.NamedObjects[objectName] as Canvas;
        }

        private void DataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid1.SelectedItem is DataRowView)
            {
                string selectedObjectName;
                Canvas newSelectedCanvas;
                DataRowView dr = DataGrid1.SelectedItem as DataRowView;

                selectedObjectName = (string)dr[0]; // first column contains the name of the linkes element

                newSelectedCanvas = GetCanvas(selectedObjectName);

                SelectElement(newSelectedCanvas);
            }
        }

        private void SelectElement(Canvas newSelectedCanvas)
        {
            if (_selectedCanvas != null)
            {
                _selectedCanvas.Effect = null; // .Net 3.5 SP1 code
                //_selectedCanvas.BitmapEffect = null; // .Net 3.0 code
            }

            if (newSelectedCanvas != null)
            {
                // DropShadowEffect requires .Net 3.5 SP1
                System.Windows.Media.Effects.DropShadowEffect dropShadowEffect = new System.Windows.Media.Effects.DropShadowEffect();
                dropShadowEffect.Opacity = 0.5;

                newSelectedCanvas.Effect = dropShadowEffect;

                // DropShadowBitmapEffect works on .Net 3.0 but is obsolete and will not work on .Net 4.0
                //System.Windows.Media.Effects.DropShadowBitmapEffect dropShadowBitmapEffect;
                //dropShadowBitmapEffect = new System.Windows.Media.Effects.DropShadowBitmapEffect();
                //dropShadowBitmapEffect.ShadowDepth = 5;

                //newSelectedCanvas.BitmapEffect = dropShadowBitmapEffect;

                _selectedCanvas = newSelectedCanvas;
            }
        }
    }
}
