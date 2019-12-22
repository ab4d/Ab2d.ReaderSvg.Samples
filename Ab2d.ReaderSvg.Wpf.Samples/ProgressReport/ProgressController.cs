// ----------------------------------------------------------------
// <copyright file="ProgressController.cs" company="Andrej Benedik s.p.">
//     Copyright (c) Andrej Benedik s.p.  All Rights Reserved
// </copyright>
// -----------------------------------------------------------------

using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Ab2d.Samples.ReaderSvgSamples.ProgressReport
{
    public class ProgressController
    {
        private delegate void NoParamsDelegate();
        private delegate void PercentageParamsDelegate(int progressPercentage);
        private delegate void StringParamsDelegate(string text);
        private delegate void ShowProgressWindowDelegate(WindowStartupLocation startupLocation, Point position, string status);

        private ProgressWindow _progressWindow;
        private bool _isShown;
        private DateTime _windowShowTime;
        private string _savedStatusText;

        private Thread _backgroundThread;


        private readonly static ProgressController _instance = new ProgressController();

        /// <summary>
        /// Static instance of ProgressController
        /// </summary>
        public static ProgressController Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Gets or sets a string that specifies a pack uri source of the icon that is used for ProgressWindow.
        /// For example for icon with build action as Resource use: "pack://application:,,,/MyIcon.ico"
        /// </summary>
        public string WindowIconSource { get; set; }


        private bool _isBackgroundThread;
        
        /// <summary>
        /// Gets or sets a boolean that specifies if ProgressWindow is shown on another (background) thread or not.
        /// Default value is true which means it is shown on another thread.
        /// </summary>
        public bool IsBackgroundThread
        {
            get { return _isBackgroundThread; }

            set
            {
                if (_isBackgroundThread == value)
                    return;

                // Cleanup
                if (_progressWindow != null)
                {
                    // when IsBackgroundThread is changed we need to close any existing progress windows and threads
                    CloseProgressWindow();

                    if (_backgroundThread != null)
                        CloseBackgroundThread();
                }

                _isBackgroundThread = value;
            }
        }


        /// <summary>
        /// Protected constructor - the class can be created only from static Instance
        /// </summary>
        protected ProgressController()
        {
            _isBackgroundThread = true; // set default value
        }


        /// <summary>
        /// Immediately shows progress window without specifying initial delay.
        /// </summary>
        public void ShowProgressWindow()
        {
            ShowProgressWindow(0);
        }

        /// <summary>
        /// Shows progress window with specifying initial delay.
        /// </summary>
        /// <param name="initialDelayInMs">initial delay in ms</param>
        public void ShowProgressWindow(int initialDelayInMs)
        {
            if (_isShown)
                return;

            // Reset the status
            _savedStatusText = null;

            // calculate the time when the window should show
            _windowShowTime = DateTime.Now.AddMilliseconds(initialDelayInMs);

            if (_progressWindow == null)
                PrepareProgressWindow();

            if (initialDelayInMs == 0 && _progressWindow != null)
                ShowProgressWindowInt();
        }

        /// <summary>
        /// Closes progress window.
        /// </summary>
        public void CloseProgressWindow()
        {
            if (_progressWindow == null || !_isShown)
                return;

            if (this.IsBackgroundThread)
                _progressWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new NoParamsDelegate(_progressWindow.HideWindow));
            else
                _progressWindow.HideWindow();

            _isShown = false;
            _savedStatusText = null;
        }

        /// <summary>
        /// Sets status text in progress window.
        /// </summary>
        /// <param name="newStatus">new status</param>
        public void SetStatus(string newStatus)
        {
            if (_progressWindow != null)
            {
                if (this.IsBackgroundThread)
                    _progressWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new StringParamsDelegate(_progressWindow.SetStatus), newStatus);
                else
                    _progressWindow.SetStatus(newStatus);
            }
            else
            {
                // window is not yet ready - save the status so it can be set when the window will be shown (in case SetStatus is called before the window is shown)
                _savedStatusText = newStatus;
            }
        }

        /// <summary>
        /// Sets progress (from 0 to 100) displayed in progress window.
        /// </summary>
        /// <param name="progressPercentage">progress from 0 to 100</param>
        public void ReportProgress(int progressPercentage)
        {
            if (_progressWindow == null)
                return;

            if (!_isShown && 
                DateTime.Now >= _windowShowTime && // is it time already to show the window
                progressPercentage < 90)           // if already more than 90% progress than do not show the window
            {
                ShowProgressWindowInt();
            }

            if (_isShown)
            {
                if (this.IsBackgroundThread)
                    _progressWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new PercentageParamsDelegate(_progressWindow.SetProgress), progressPercentage);
                else
                    _progressWindow.SetProgress(progressPercentage);
            }
        }

        /// <summary>
        /// Creates an instance of the progress window. If IsBackgroundThread is true, the method also prepares background thread.
        /// This method can be called to prepare the background thread and progress window so the window can be shown quicker.
        /// </summary>
        public void PrepareProgressWindow()
        {
            if (_progressWindow != null)
                return;

            if (this.IsBackgroundThread)
            {
                if (_backgroundThread != null)
                    return;

                // Create background thread that will show our progress window
                _backgroundThread = new Thread(CreateProgressWindow);
                _backgroundThread.SetApartmentState(ApartmentState.STA); // WPF only runs on STA
                _backgroundThread.IsBackground = true;
                _backgroundThread.Start(); // Start the thread - prepare a Progress window and start a dispatcher (so the window will be prepared and can quickly open)
            }
            else
            {
                CreateProgressWindow();
            }
        }

        /// <summary>
        /// The method can be called to close the background thread. If this method is not called any started thread will be closed automatically when the main process will terminate.
        /// </summary>
        public void CloseBackgroundThread()
        {
            if (_progressWindow == null)
                return;

            if (this.IsBackgroundThread)
            {
                if (_backgroundThread != null)
                {
                    _progressWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new NoParamsDelegate(_progressWindow.Close));

                    // Shut down dispatcher with lower priority so the window can close, etc.
                    _progressWindow.Dispatcher.BeginInvokeShutdown(DispatcherPriority.ApplicationIdle);

                    _progressWindow = null;

                    // The current thread will close when the dispatcher is shut down
                    // So we can mark the thread as closed
                    _backgroundThread = null;
                }
            }
            else
            {
                _progressWindow.Close();
                _progressWindow = null;
            }
        }


        private void ShowProgressWindowInt()
        {
            if (_progressWindow == null || _isShown)
                return;


            if (this.IsBackgroundThread)
            {
                // The progress window will be shown in in another thread - use the mainWindowBounds to get the startup location

                System.Windows.WindowStartupLocation windowStartupLocation;
                Point position;
                Rect mainWindowBounds;
                Window mainWindow;

                mainWindow = Application.Current.MainWindow;

                if (mainWindow != null)
                    mainWindowBounds = new Rect(new Point(mainWindow.Left, mainWindow.Top), new Size(mainWindow.ActualWidth, mainWindow.ActualHeight));
                else
                    mainWindowBounds = Rect.Empty; // No MainWindow

                if (double.IsNaN(mainWindowBounds.Width) || mainWindowBounds.Width == 0 || double.IsNaN(mainWindowBounds.Height) || mainWindowBounds.Height == 0)
                {
                    // mainWindowBounds not set or not correct - center screen
                    windowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    position = new Point();
                }
                else
                {
                    // Manually center to MainWindow
                    windowStartupLocation = System.Windows.WindowStartupLocation.Manual;
                    position = new Point((mainWindowBounds.Left + mainWindowBounds.Width / 2) - ProgressWindow.WINDOW_WIDTH / 2,
                                         (mainWindowBounds.Top + mainWindowBounds.Height / 2) - ProgressWindow.WINDOW_HEIGHT / 2);
                }

                _progressWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ShowProgressWindowDelegate(_progressWindow.ShowProgressWindow), windowStartupLocation, position, _savedStatusText);
            }
            else
            {
                // Because we are running in the same thread we can set location to CenterOwner
                _progressWindow.Owner = System.Windows.Application.Current.MainWindow;
                _progressWindow.ShowProgressWindow(System.Windows.WindowStartupLocation.CenterOwner, new Point(), _savedStatusText);
            }

            _isShown = true;
        }

        // This method can be called on another thread (if IsBackgroundThread is true)
        private void CreateProgressWindow()
        {
            if (_progressWindow != null)
                return;


            // Create an instance of ProgressWindow
            _progressWindow = new ProgressWindow();

            // Set progress window icon if seource is set
            if (!string.IsNullOrEmpty(this.WindowIconSource))
                _progressWindow.Icon = System.Windows.Media.Imaging.BitmapFrame.Create(new Uri(this.WindowIconSource, UriKind.RelativeOrAbsolute));

            if (this.IsBackgroundThread)
            {
                // Start the dispatcher for this thread
                // This method waits until the Dispatcher is shut down
                System.Windows.Threading.Dispatcher.Run();

                // after Run method finishes the CreateProgressWindow will also finish and this will close the thread 
            }
        }
    }
}
