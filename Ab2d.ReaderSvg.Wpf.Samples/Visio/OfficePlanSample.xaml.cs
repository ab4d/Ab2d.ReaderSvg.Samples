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

namespace Ab2d.Samples.ReaderSvgSamples.Visio
{
    /// <summary>
    /// Interaction logic for OfficePlanSample.xaml
    /// </summary>
    public partial class OfficePlanSample : UserControl
    {
        public OfficePlanSample()
        {
            InitializeComponent();

            CollectLayers();
            CollectDataElements();
        }

        private void CollectDataElements()
        {
            List<object> objectsWithCustomProperties = SvgOfficePlan.InnerReaderSvg.GetObjectsWithCustomProperties();

            foreach (object oneObjectWithData in objectsWithCustomProperties)
            {
                UIElement oneUIElement;

                oneUIElement = (UIElement)oneObjectWithData;

                oneUIElement.MouseEnter += new MouseEventHandler(oneUIElement_MouseEnter);
                oneUIElement.MouseLeave += new MouseEventHandler(oneUIElement_MouseLeave);
                oneUIElement.MouseMove += new MouseEventHandler(oneUIElement_MouseMove);
            }
        }

        void oneUIElement_MouseMove(object sender, MouseEventArgs e)
        {
            UpdateCustomDataPosition();
        }

        void oneUIElement_MouseLeave(object sender, MouseEventArgs e)
        {
            CustomDataBorder.Visibility = Visibility.Collapsed;
        }

        void oneUIElement_MouseEnter(object sender, MouseEventArgs e)
        {
            Dictionary<string, object> customProperties;

            customProperties = SvgOfficePlan.InnerReaderSvg.GetCustomProperties(sender);

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

        private void CollectLayers()
        {
            List<string> layers;

            // Get names of the layers defined in Visio
            layers = SvgOfficePlan.InnerReaderSvg.GetLayerNames();

            foreach (string oneLayerName in layers)
            {
                CheckBox newCheckBox;

                newCheckBox = new CheckBox();
                newCheckBox.Content = oneLayerName;
                newCheckBox.IsChecked = true;
                newCheckBox.Checked += new RoutedEventHandler(LayerRadioButton_Checked);
                newCheckBox.Unchecked += new RoutedEventHandler(LayerRadioButton_Checked);

                LayersPanel.Children.Add(newCheckBox);
            }
        }

        void LayerRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            string layerName;
            CheckBox changedCheckBox;

            changedCheckBox = (CheckBox)sender;

            layerName = (string)changedCheckBox.Content;

            ShowHideLayerElements(layerName, changedCheckBox.IsChecked ?? false);
        }

        private void ShowHideLayerElements(string layerName, bool isVisible)
        {
            List<UIElement> elementsForLayerName = SvgOfficePlan.InnerReaderSvg.GetElementsForLayerName(layerName);

            // TODO:
            // Because one object can be in multiple layers 
            // we should check that when showing an object 
            // all the layers for the objects should be visible

            foreach (UIElement oneElement in elementsForLayerName)
            {
                if (isVisible)
                    oneElement.Visibility = Visibility.Visible;
                else
                    oneElement.Visibility = Visibility.Collapsed;
            }
        }
    }
}
