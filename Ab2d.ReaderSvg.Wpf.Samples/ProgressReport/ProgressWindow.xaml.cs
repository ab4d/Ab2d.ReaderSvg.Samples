// ----------------------------------------------------------------
// <copyright file="ProgressWindow.xaml.cs" company="Andrej Benedik s.p.">
//     Copyright (c) Andrej Benedik s.p.  All Rights Reserved
// </copyright>
// -----------------------------------------------------------------

// ----------------------------------------------------------------
// <copyright file="ProgressController.cs" company="AB4D d.o.o.">
//     Copyright (c) AB4D d.o.o.  All Rights Reserved
// </copyright>
// -----------------------------------------------------------------

using System;
using System.Windows;

namespace Ab2d.Samples.ReaderSvgSamples.ProgressReport
{
    /// <summary>
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {
        // Define constants that can be accessed from another thread
        public const double WINDOW_WIDTH = 300;
        public const double WINDOW_HEIGHT = 100;

        public ProgressWindow()
        {
            InitializeComponent();

            this.Width = WINDOW_WIDTH;
            this.Height = WINDOW_HEIGHT;
        }

        public void ShowProgressWindow(System.Windows.WindowStartupLocation startupLocation, Point position, string status)
        {
            this.WindowStartupLocation = startupLocation;

            if (startupLocation == System.Windows.WindowStartupLocation.Manual)
            {
                this.Left = position.X;
                this.Top = position.Y;
            }

            // the status value is passed to this method only if the SetStatus was called before the progress window was created
            if (!string.IsNullOrEmpty(status))
                SetStatus(status);

            this.Show();
        }

        public void HideWindow()
        {
            // Reset the values
            ProgressBar1.Value = 0;
            StatusTextBlock.Text = "";

            this.Hide();
        }

        public void SetProgress(int progressPercentage)
        {
            ProgressBar1.Value = progressPercentage;
        }

        public void SetStatus(string newStatus)
        {
            StatusTextBlock.Text = newStatus;
        }
    }
}
