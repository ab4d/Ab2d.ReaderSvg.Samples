using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Ab2d.Samples.ReaderSvgSamples.UseCases
{
    public partial class UseCasesIntroductionPage : UserControl
    {
        public UseCasesIntroductionPage()
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