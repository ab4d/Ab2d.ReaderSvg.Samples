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
using System.Windows.Shapes;


namespace Ab2d.Samples.ReaderSvgSamples.Controls
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class SvgViewboxSample : UserControl
    {
        public SvgViewboxSample()
        {
            // Copy the home11.svg to local folder
            var localSvgFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "home11.svg");
            if (!System.IO.File.Exists(localSvgFilePath))
            {
                var streamResourceInfo = Application.GetResourceStream(new Uri("pack://application:,,,/Resources/home11.svg"));
                if (streamResourceInfo != null)
                {
                    using (var fileStream = System.IO.File.Create(localSvgFilePath))
                        streamResourceInfo.Stream.CopyTo(fileStream);
                }
            }

            InitializeComponent();
        }

        public void PageLoaded(object sender, RoutedEventArgs e)
        {
            string localSvgFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\paint.svg");

            LocalFileSvgViewbox.Source = new Uri(localSvgFilePath);
            LocalFileSvgTextBlock.Text = string.Format("<ab2d:SvgViewbox Source=\"{0}\"/>", localSvgFilePath);

            var streamResourceInfo = Application.GetResourceStream(new Uri("pack://application:,,,/Resources/home1.svg"));
            if (streamResourceInfo != null)
                SvgViewboxFromStream.SourceStream = streamResourceInfo.Stream;
        }

        public void LoadSvgFromWebButtonClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                WebSvgViewbox.Source = new Uri("https://www.ab4d.com/images/tiger.svg");

                LoadSvgFromWebButton.Visibility = Visibility.Collapsed;
                WebSvgViewbox.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception reading web content", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        
    }
}