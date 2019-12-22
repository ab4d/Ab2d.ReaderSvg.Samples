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

namespace Ab2d.Samples.ReaderSvgSamples.UseCases
{
    /// <summary>
    /// Interaction logic for SvgLayoutSample.xaml
    /// </summary>

    public partial class SvgLayoutSample : UserControl
    {
        //Ab2d.ReaderSvg _myReaderSvg;

        Canvas _playButton;
        Canvas _pauseButton;
        Canvas _stopButton;
        Shape _clockHand;

        bool _isClockPaused = false;
        //bool _isInitialized = false;

        public SvgLayoutSample()
        {
            InitializeComponent();

            // Create a NameScope for the page so
            // that Storyboards can be used.
            NameScope.SetNameScope(this, new NameScope());

            this.Loaded += new RoutedEventHandler(SvgLayoutSample_Loaded);
        }

        void SvgLayoutSample_Loaded(object sender, RoutedEventArgs e)
        {
            RegisterElements();
        }

        //
        // OLD SOLUTION without SvgViewbox (using ReaderSvg directly):
        //

        //public void PageLoaded(object sender, RoutedEventArgs e)
        //{
        //    if (!_isInitialized)
        //        LoadSvg(AppDomain.CurrentDomain.BaseDirectory + @"samples\svg_layout.svg");
        //}

        //private void LoadSvg(string fileName)
        //{
        //    Viewbox importedElement;

        //    // Create an instance of ReaderSvg so we can get the NamedObjects dictionary - access the elements by ids
        //    _myReaderSvg = new Ab2d.ReaderSvg();

        //    // Reads the svg file
        //    importedElement = _myReaderSvg.Read(fileName);

        //    MainSvgGrid.Children.Add(importedElement);

        //    RegisterElements();

        //    _isInitialized = true;
        //}

        private void RegisterElements()
        {
            // Add mouse events to appropriate elements from svg file - accessed by id (defined in drawing app)

            _playButton  = SvgUserInterfaceViewbox.InnerReaderSvg.NamedObjects["PlayButton"] as Canvas;
            _pauseButton = SvgUserInterfaceViewbox.InnerReaderSvg.NamedObjects["PauseButton"] as Canvas;
            _stopButton  = SvgUserInterfaceViewbox.InnerReaderSvg.NamedObjects["StopButton"] as Canvas;
            _clockHand   = SvgUserInterfaceViewbox.InnerReaderSvg.NamedObjects["ClockHand"] as Shape;

            _playButton.MouseEnter += new MouseEventHandler(SvgLayoutSample_MouseEnter);
            _pauseButton.MouseEnter += new MouseEventHandler(SvgLayoutSample_MouseEnter);
            _stopButton.MouseEnter += new MouseEventHandler(SvgLayoutSample_MouseEnter);

            _playButton.MouseLeave += new MouseEventHandler(SvgLayoutSample_MouseLeave);
            _pauseButton.MouseLeave += new MouseEventHandler(SvgLayoutSample_MouseLeave);
            _stopButton.MouseLeave += new MouseEventHandler(SvgLayoutSample_MouseLeave);

            _playButton.MouseUp += new MouseButtonEventHandler(SvgLayoutSample_PlayMouseUp);
            _pauseButton.MouseUp += new MouseButtonEventHandler(SvgLayoutSample_PauseMouseUp);
            _stopButton.MouseUp += new MouseButtonEventHandler(SvgLayoutSample_StopMouseUp);

            Rect clockHandBound = _clockHand.RenderedGeometry.Bounds;
            _clockHand.Fill = Brushes.Red; // In evaluation the hand can be replayed by demo test - use original Red color instead

            RotateTransform clockRotate = new RotateTransform(0, clockHandBound.X + clockHandBound.Width / 2, clockHandBound.Y + clockHandBound.Height*0.95);
            this.RegisterName("ClockRotate", clockRotate);

            _clockHand.RenderTransform = clockRotate;
        }

        void SvgLayoutSample_PlayMouseUp(object sender, MouseButtonEventArgs e)
        {
            Storyboard clockAnimationStoryboard = (Storyboard)this.FindResource("ClockAnimation");
            Storyboard.SetTargetName(clockAnimationStoryboard, "ClockRotate");

            if (_isClockPaused)
                clockAnimationStoryboard.Resume(this);
            else
                clockAnimationStoryboard.Begin(this, true);

            _isClockPaused = false;
        }

        void SvgLayoutSample_PauseMouseUp(object sender, MouseButtonEventArgs e)
        {
            Storyboard clockAnimationStoryboard = (Storyboard)this.FindResource("ClockAnimation");
            clockAnimationStoryboard.Pause(this);

            _isClockPaused = true;
        }

        void SvgLayoutSample_StopMouseUp(object sender, MouseButtonEventArgs e)
        {
            Storyboard clockAnimationStoryboard = (Storyboard)this.FindResource("ClockAnimation");
            clockAnimationStoryboard.Stop(this);

            _isClockPaused = false;
        }

        void SvgLayoutSample_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Canvas)
            {
                Canvas currentCanvas = sender as Canvas;
                Shape selectedShape = currentCanvas.Children[0] as Shape;
                selectedShape.Fill = new SolidColorBrush(Color.FromRgb(179, 179, 179));
                this.Cursor = Cursors.Arrow;
            }
        }

        void SvgLayoutSample_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Canvas)
            {
                Canvas currentCanvas = sender as Canvas;
                Shape selectedShape = currentCanvas.Children[0] as Shape;
                selectedShape.Fill = new SolidColorBrush(Color.FromRgb(255, 233, 0));
                this.Cursor = Cursors.Hand;
            }
        }
    }
}