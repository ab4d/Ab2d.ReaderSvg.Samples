using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Ab2d.Samples.ReaderSvgSamples.ProgressReport
{
    public partial class ProgressIntroductionPage : UserControl
    {
        public ProgressIntroductionPage()
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