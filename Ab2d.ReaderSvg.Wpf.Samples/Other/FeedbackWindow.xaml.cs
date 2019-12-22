using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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

namespace Ab2d.Samples.ReaderSvgSamples.Other
{
    /// <summary>
    /// Interaction logic for FeedbackWindow.xaml
    /// </summary>
    public partial class FeedbackWindow : Window
    {
        public class WebRequestData
        {
            public string Url;
            public string Method;
            public string MimeType;
            public byte[] PostByteArray;
            public string ResponseText;
        }

        private bool _isDataSent;
        private System.ComponentModel.BackgroundWorker _uploadDataWorker;

        public FeedbackWindow()
        {
            InitializeComponent();

            this.Loaded += delegate(object sender, RoutedEventArgs args)
            {
                MessageTextBlock.Select(MessageTextBlock.Text.Length, 1);
                MessageTextBlock.Focus();
            };
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isDataSent)
                this.Close();
            else
                SendMessage();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private string CreateMessage()
        {
            StringBuilder sb;

            sb = new StringBuilder();

            sb.Append("Ab2d.ReaderSvg.Samples feedback").AppendLine();
            sb.Append("v1.0").AppendLine();
            sb.AppendFormat("Date: {0:yyyy}-{0:MM}-{0:dd} {0:HH}:{0:mm}:{0:ss}", DateTime.Now).AppendLine();
            sb.AppendFormat("Ab2d.ReaderSvg version:{0}", typeof(Ab2d.ReaderSvg).Assembly.GetName().Version.ToString()).AppendLine();
            sb.AppendLine();
            //sb.AppendFormat("User email: {0}", EMailTextBlock.Text).AppendLine();
            sb.Append("Message:").AppendLine();
            sb.AppendLine(MessageTextBlock.Text);

            return sb.ToString();
        }

        private void SendMessage()
        {
            string url;
            string messageToSend;
            byte[] postByteArray;

            if (MessageTextBlock.Text == "")
                return;

            messageToSend = CreateMessage();
            postByteArray = Encoding.Unicode.GetBytes(messageToSend);

            url = "http://www.ab4d.com/ApplicationFeedback.aspx?source=Ab2d.ReaderSvg";

            SendButton.IsEnabled = false;
            ProgressPanel.Visibility = Visibility.Visible;
            ProgressBar1.IsIndeterminate = true;


            WebRequestData webRequestData = new WebRequestData();
            webRequestData.Url = url;
            webRequestData.Method = "POST";
            webRequestData.MimeType = "application/octet-stream"; // undeterminied binary // See http://www.hansenb.pdx.edu/DMKB/dict/tutorials/mime_typ.php
            webRequestData.PostByteArray = postByteArray;

            _uploadDataWorker = new System.ComponentModel.BackgroundWorker();
            _uploadDataWorker.WorkerReportsProgress = false;
            _uploadDataWorker.WorkerSupportsCancellation = true;
            _uploadDataWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(uploadDataWorker_DoWork);
            _uploadDataWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(uploadDataWorker_RunWorkerCompleted);

            _uploadDataWorker.RunWorkerAsync(webRequestData);

            System.Windows.Threading.DispatcherTimer timeoutTimer = new System.Windows.Threading.DispatcherTimer();

            timeoutTimer.Tick += delegate(object s, EventArgs args)
            {
                _uploadDataWorker.CancelAsync();
            };

            timeoutTimer.Interval = new TimeSpan(0, 2, 0); // timeout = 2 min
            timeoutTimer.Start();
        }

        void uploadDataWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                WebRequestData webRequestData = e.Argument as WebRequestData;

                WebClient client = new WebClient();

                client.Headers.Add("Content-Type", webRequestData.MimeType);

                byte[] responseArray = client.UploadData(webRequestData.Url, webRequestData.Method, webRequestData.PostByteArray);
                e.Result = Encoding.UTF8.GetString(responseArray);
            }
            catch
            {
                e.Result = null;
            }
        }

        void uploadDataWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            string responseText;

            if (e.Cancelled)
            {
                ProgressTextBlock.Text = "Timeout expired!";
            }
            else
            {
                responseText = e.Result as string;

                if (responseText == null || responseText.Contains("Data successfully sent!"))
                {
                    ProgressTextBlock.Text = "Data successfully sent. Thank you for sending feedback.";

                    _isDataSent = true;
                    SendButton.Content = "Close";
                }
                else
                {
                    ProgressTextBlock.Text = "Failed to send data";
                }
            }

            ProgressBar1.IsIndeterminate = false;
            SendButton.IsEnabled = true;
        }
    }
}
