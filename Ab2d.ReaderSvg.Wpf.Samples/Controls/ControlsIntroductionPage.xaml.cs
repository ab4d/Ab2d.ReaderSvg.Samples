using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Ab2d.Samples.ReaderSvgSamples.Controls
{
    public partial class ControlsIntroductionPage : UserControl
    {
        public ControlsIntroductionPage()
        {
            InitializeComponent();
        }

        private void link_navigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
            e.Handled = true;
        }
    }
}