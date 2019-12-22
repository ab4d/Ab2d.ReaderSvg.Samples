using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Ab2d.Samples.ReaderSvgSamples.Visio
{
    public partial class VisioIntroductionPage : UserControl
    {
        public VisioIntroductionPage()
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