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
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Ab2d.Samples.ReaderSvgSamples.Visio
{
    /// <summary>
    /// Interaction logic for DynamicVisioFromXaml.xaml
    /// </summary>
    public partial class DynamicVisioFromXaml : UserControl
    {
        private List<UIElement> _stepElements;
        private List<UIElement> _arrowElements;

        private bool _isAnimationStarted;
        private DispatcherTimer _timer;

        private Brush _savedFillBrush;



        public DynamicVisioFromXaml()
        {
            InitializeComponent();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += new EventHandler(_timer_Tick);

            CollectKnonwElements();
            StartAnimation();
        }

        private void CollectKnonwElements()
        {
            List<string> stepNames;
            List<string> arrowNames;

            stepNames = new List<string>();
            arrowNames = new List<string>();

            // Collect the names of the elements showins the steps and arrows
            foreach (UIElement oneElement in Page_1.Children)
            {
                if (oneElement is Canvas)
                {
                    string name;

                    name = ((Canvas)oneElement).Name;

                    if (name.StartsWith("Step"))
                        stepNames.Add(name);
                    else if (name.StartsWith("Arrow"))
                        arrowNames.Add(name);
                }
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

                uiElement = (UIElement)Page_1.FindName(oneName);

                // Show some "mouse over" effect
                uiElement.MouseEnter += new MouseEventHandler(uiElement_MouseEnter);
                uiElement.MouseLeave += new MouseEventHandler(uiElement_MouseLeave);

                _stepElements.Add(uiElement);
            }


            _arrowElements = new List<UIElement>();

            foreach (string oneName in arrowNames)
                _arrowElements.Add((UIElement)Page_1.FindName(oneName));


            StepSlider.Maximum = (_stepElements.Count > arrowNames.Count) ? _stepElements.Count + 1: arrowNames.Count + 1;
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
