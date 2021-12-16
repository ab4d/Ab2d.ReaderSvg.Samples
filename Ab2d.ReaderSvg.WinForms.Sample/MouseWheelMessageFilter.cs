using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;

namespace ReaderSvg.WinFormsSample
{
    // Based on source from http://stackoverflow.com/questions/5723290/mouse-events-are-not-received-by-a-wpf-scrollviewer-when-hosted-in-a-winforms-co
    public class MouseWheelMessageFilter : IMessageFilter
    {
        private const int WM_MOUSEWHEEL = 0x020A;
        private FrameworkElement _element;

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        public static void RegisterMouseWheelHandling(FrameworkElement element)
        {
            var mouseWheelMessageFilter = new MouseWheelMessageFilter(element);
        }

        private MouseWheelMessageFilter(FrameworkElement element)
        {
            _element = element;

            _element.Loaded += delegate(object sender, RoutedEventArgs args)
            {
                System.Windows.Forms.Application.AddMessageFilter(this);
            };

            _element.Unloaded += delegate(object sender, RoutedEventArgs args)
            {
                System.Windows.Forms.Application.RemoveMessageFilter(this);
            };
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (!_element.IsVisible)
                return false;

            if (m.Msg == WM_MOUSEWHEEL)
            {
                Rect rect = new Rect(0, 0, _element.ActualWidth, _element.ActualHeight);
                System.Windows.Point pt = Mouse.GetPosition(_element);

                if (rect.Contains(pt))
                {
                    HwndSource hwndSource = (HwndSource)HwndSource.FromVisual(_element);
                    SendMessage(hwndSource.Handle, m.Msg, m.WParam, m.LParam);
                    return true;
                }
            }

            return false;
        }
    }
}