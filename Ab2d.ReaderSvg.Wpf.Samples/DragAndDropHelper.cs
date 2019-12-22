using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Ab2d.Samples.ReaderSvgSamples
{
    public class FileDropedEventArgs : EventArgs
    {
        public string FileName;

        public FileDropedEventArgs(string fileName)
        {
            FileName = fileName;
        }
    }

    public class DragAndDropHelper
    {
        private FrameworkElement _eventSource;
        private string[] _allowedFileExtensions;

        public event EventHandler<FileDropedEventArgs> FileDroped;

        public DragAndDropHelper(FrameworkElement eventSource, string allowedFileExtensions)
        {
            _eventSource = eventSource;
            _allowedFileExtensions = allowedFileExtensions.Split(';');

            eventSource.AllowDrop = true;
            eventSource.Drop += new System.Windows.DragEventHandler(pageToAddDragAndDrop_Drop);
            eventSource.DragOver += new System.Windows.DragEventHandler(pageToAddDragAndDrop_DragOver);

        }

        public void pageToAddDragAndDrop_DragOver(object sender, DragEventArgs e)
        {
            string fileName;

            e.Effects = DragDropEffects.None;
            e.Handled = true;

            if (e.Data.GetDataPresent("FileNameW"))
            {
                object dropData;
                string[] dropFileNames;
                string fileExtension;

                dropData = e.Data.GetData("FileNameW");

                if (dropData is string[])
                {
                    dropFileNames = dropData as string[];

                    fileName = dropFileNames[0].ToString(); // Get only the first file name
                    fileExtension = System.IO.Path.GetExtension(fileName).ToLower();

                    foreach (string oneFileFilter in _allowedFileExtensions)
                    {
                        if (fileExtension == oneFileFilter)
                        {
                            e.Effects = DragDropEffects.Move;
                            break;
                        }
                    }
                }
            }
        }

        public void pageToAddDragAndDrop_Drop(object sender, DragEventArgs e)
        {
            string fileName;

            if (e.Data.GetDataPresent("FileNameW"))
            {
                object dropData;
                string[] dropFileNames;

                dropData = e.Data.GetData("FileNameW");

                if (dropData is string[])
                {
                    dropFileNames = dropData as string[];

                    fileName = dropFileNames[0].ToString(); // Get only the first file name

                    if (FileDroped != null)
                        FileDroped(this, new FileDropedEventArgs(fileName));
                }
            }
        }
    }
}
