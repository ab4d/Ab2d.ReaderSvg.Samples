using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Ab2d.Samples.ReaderSvgSamples.Other
{
    public partial class IntroductionPage : UserControl
    {
        public IntroductionPage()
        {
            InitializeComponent();

			EvaluationImage.Visibility = Visibility.Collapsed;
			EvaluationTextBlockEx.Visibility = Visibility.Collapsed;
        }
    }
}