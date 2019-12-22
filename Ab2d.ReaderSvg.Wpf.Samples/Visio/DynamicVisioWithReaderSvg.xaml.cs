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
using System.Windows.Threading;

namespace Ab2d.Samples.ReaderSvgSamples.Visio
{
    /// <summary>
    /// Interaction logic for DynamicVisioWithReaderSvg.xaml
    /// </summary>
    public partial class DynamicVisioWithReaderSvg : UserControl
    {
        private List<UIElement> _stepElements;
        private List<UIElement> _arrowElements;

        private bool _isAnimationStarted;
        private DispatcherTimer _timer;

        private Brush _savedFillBrush;

        public DynamicVisioWithReaderSvg()
        {
            InitializeComponent();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += new EventHandler(_timer_Tick);

            CollectKnownElements();
            StartAnimation();
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
            List<UIElement> elementsForLayerName = SvgNetwork.InnerReaderSvg.GetElementsForLayerName(layerName);

            foreach (UIElement oneElement in elementsForLayerName)
            {
                if (isVisible)
                    oneElement.Visibility = Visibility.Visible;
                else
                    oneElement.Visibility = Visibility.Collapsed;
            }
        }



        private void CollectKnownElements()
        {
            List<string> stepNames;
            List<string> arrowNames;

            stepNames = new List<string>();
            arrowNames = new List<string>();

            // Collect the names of the elements showins the steps and arrows
            foreach (string oneObjectName in SvgNetwork.NamedObjects.Keys)
            {
                if (oneObjectName.StartsWith("Step"))
                    stepNames.Add(oneObjectName);
                else if (oneObjectName.StartsWith("Arrow"))
                    arrowNames.Add(oneObjectName);
            }

            // The elements are not defined in the order as the drawn process descibe them
            // But their names can be sorted to the the correct element order
            stepNames.Sort();
            arrowNames.Sort();



            // Now add elements in correct order to collections
            _stepElements = new List<UIElement>();

            foreach (string oneName in stepNames)
            {
                UIElement uiElement;

                uiElement = (UIElement)SvgNetwork.NamedObjects[oneName];

                // Show some "mouse over" effect
                uiElement.MouseEnter += new MouseEventHandler(uiElement_MouseEnter);
                uiElement.MouseLeave += new MouseEventHandler(uiElement_MouseLeave);

                _stepElements.Add(uiElement);
            }


            _arrowElements = new List<UIElement>();

            foreach (string oneName in arrowNames)
                _arrowElements.Add((UIElement)SvgNetwork.NamedObjects[oneName]);


            StepSlider.Maximum = (_stepElements.Count > arrowNames.Count) ? _stepElements.Count + 1 : arrowNames.Count + 1;
        }

        void uiElement_MouseLeave(object sender, MouseEventArgs e)
        {
            Shape childShape;

            childShape = ((Canvas)sender).Children[0] as Shape;

            childShape.Fill = _savedFillBrush;
        }

        void uiElement_MouseEnter(object sender, MouseEventArgs e)
        {
            Shape childShape;

            childShape = ((Canvas)sender).Children[0] as Shape;

            _savedFillBrush = childShape.Fill;
            childShape.Fill = Brushes.LightBlue;
        }

        private void GoToStep(int stepIndex)
        {
            Visibility newVisibility;

            stepIndex--; // so stepIndex == 0 will hide all the elements

            // First show all elements until stepIndex
            for (int i = 0; i < _arrowElements.Count + 1; i++) // +1 becasue we decreased stepIndex
            {
                if (i <= stepIndex)
                    newVisibility = Visibility.Visible;
                else
                    newVisibility = Visibility.Collapsed;


                if (i < _stepElements.Count)
                    _stepElements[i].Visibility = newVisibility;

                // Arrow with index == 0 will be shown when step element with index == 1 is shown
                if (i > 0 && i < _arrowElements.Count + 1)
                    _arrowElements[i - 1].Visibility = newVisibility;
            }
        }

        private void PlayStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isAnimationStarted)
                StopAnimation();
            else
                StartAnimation();
        }

        private void StartAnimation()
        {
            StepSlider.Value = 1;
            _timer.Start();

            PlayStopButton.Content = "Stop";

            _isAnimationStarted = true;
        }

        private void StopAnimation()
        {
            _timer.Stop();

            PlayStopButton.Content = "Play";

            _isAnimationStarted = false;
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            if (StepSlider.Value >= StepSlider.Maximum)
                StopAnimation();
            else
                StepSlider.Value += 1;
        }

        private void StepSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GoToStep(Convert.ToInt32(e.NewValue));
        }
    }
}
