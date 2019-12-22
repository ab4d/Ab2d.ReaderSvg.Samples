using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using Ab2d.Samples.ReaderSvgSamples.Other;

namespace Ab2d.Samples.ReaderSvgSamples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // To activate the commercial license uncomment and fill the following line of code (see your User Account web page for more info):
            //Ab2d.Licensing.ReaderSvg.LicenseHelper.SetLicense(licenseOwner: "[CompanyName]", 
            //                                                  licenseType: "[LicenseType]", 
            //                                                  license: "[LicenseText]");

            InitializeComponent();
        }

        public void ShowSupportPage()
        {
            var supportPageElement = SampleList.Items.OfType<System.Xml.XmlElement>()
                                                     .First(x => x.Attributes["Page"] != null && x.Attributes["Page"].Value == "Other/SupportPage.xaml");

            SampleList.SelectedItem = supportPageElement;
            SampleListScrollViewer.ScrollToEnd();
        }

        private void TextBlock_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            string newDescriptionText = "";

            var node = e.NewValue as System.Xml.XmlNode;

            if (node != null && node.Attributes != null)
            {
                System.Xml.XmlAttribute attribute = node.Attributes["Description"];

                if (attribute != null)
                    newDescriptionText = attribute.Value;
            }

            DescriptionTextBlock.ContentText = newDescriptionText;
        }

        private void LogoImage_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://www.ab4d.com");
        }

        //private void ContentFrame_OnNavigated(object sender, NavigationEventArgs e)
        //{
        //    // Prevent navigation (for example clicking back button) because our ListBox is not updated when this navigation occurs
        //    // We prevent navigation with clearing the navigation history each time navigation item changes
        //    ContentFrame.NavigationService.RemoveBackEntry();
        //}
    }
}
