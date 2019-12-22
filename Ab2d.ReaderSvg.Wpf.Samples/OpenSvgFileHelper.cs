using System;

namespace Ab2d.Samples.ReaderSvgSamples
{
    public class OpenSvgFileHelper
    {
        public static string ShowOpenFileDialog()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog;

            openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.DefaultExt = "svg";
            openFileDialog.Filter = "svg files (*.svg)|*.svg";
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "Select svg file to open";
            openFileDialog.InitialDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Resources\"));
            openFileDialog.ValidateNames = true;

            if (openFileDialog.ShowDialog() ?? false)
                return openFileDialog.FileName;

            return null;
        }
    }
}