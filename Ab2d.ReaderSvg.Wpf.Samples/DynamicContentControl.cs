// ----------------------------------------------------------------
// <copyright file="DynamicContentControl.cs" company="AB4D d.o.o.">
//     Copyright (c) AB4D d.o.o.  All Rights Reserved
// </copyright>
// -----------------------------------------------------------------

// License note:
// You may use this control free of charge and for any project you wish. Just do not blame me for any problems with the control.


using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Resources;

namespace Ab2d.Samples.ReaderSvgSamples
{
    /// <summary>
    /// DynamicContentControl can be used to dynamically set the control that is shown by specifying the control type name or image resource name.
    /// </summary>
    [ContentProperty("Source")]
    public class DynamicContentControl : ContentControl
    {
        #region Source
        /// <summary>
        /// Gets or sets the type name of a WPF control (for example UserControl) or name of jpg or png image from resources that will be shown with this DynamicContentControl.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Source</b> gets or sets the type name of a WPF control (for example UserControl) that will be shown with this DynamicContentControl.
        /// </para>
        /// <para>
        /// Source should be set to the full type name (namespace and type name) of the UserControl or some other control that can be shown as Content of ContentControl.
        /// </para>
        /// <para>
        /// When Source does not contain the full type name, the DynamicContentControl uses the <see cref="BaseNamespace"/> property to get full type name: fullTypeName = BaseNamespace + '.' + Source
        /// </para>
        /// <para>
        /// If BaseNamespace is not set, then the DynamicContentControl tries to find the full type name by first using the name of the currently executing assembly.
        /// If this still does not work, the entry assembly name is used.
        /// </para>
        /// </remarks>
        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// DependencyProperty
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(DynamicContentControl),
                new PropertyMetadata(null, OnSourcePropertyChanged));

        static void OnSourcePropertyChanged(DependencyObject obj,
                                            DependencyPropertyChangedEventArgs args)
        {
            ((DynamicContentControl)obj).UpdateContent();
        }
        #endregion

        /// <summary>
        /// Gets or sets a string that specifies the base namespace name - BaseNamespace is combined with Source property value to get full type name of a UserControl that will be shown.
        /// </summary>
        public string BaseNamespace { get; set; }

        private void UpdateContent()
        {
            string source = this.Source; // Local accessor

            if (string.IsNullOrEmpty(source))
            {
                this.Content = null;
                return;
            }


            string lowerSource = source.ToLower();

            if (lowerSource.EndsWith(".jpg") || lowerSource.EndsWith(".png"))
            {
                ShowImage(source);
                return;
            }

            Type type = GetTypeFromSource(source);

            if (type != null)
            {
                try
                {
                    var newContent = Activator.CreateInstance(type);
                    this.Content = newContent;
                }
                catch (Exception ex)
                {
                    ShowErrorTextBlock("Cannot create an instance of type: " + source);
                }
            }
            else
            {
                ShowErrorTextBlock("Cannot get full type name for Source: " + this.Source);
            }
        }


        private void ShowImage(string imageSource)
        {
            string resourceName = GetResourceName(Assembly.GetExecutingAssembly(), imageSource);

            StreamResourceInfo streamResourceInfo;

            try
            {
                streamResourceInfo = Application.GetResourceStream(new Uri(resourceName));
            }
            catch
            {
                streamResourceInfo = null;
            }

            if (streamResourceInfo == null)
            {
                resourceName = GetResourceName(Assembly.GetEntryAssembly(), imageSource);
                try
                {
                    streamResourceInfo = Application.GetResourceStream(new Uri(resourceName));
                }
                catch
                { }
            }

            if (streamResourceInfo == null)
            {
                ShowErrorTextBlock("Cannot find Resource: " + this.Source);
                return;
            }

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = streamResourceInfo.Stream;
            bitmap.EndInit();

            var image = new Image()
            {
                Source = bitmap,
                Stretch = Stretch.None,
                SnapsToDevicePixels = true,
                UseLayoutRounding = true // This and NearestNeighbor produces the most crisp image in WPF when image is shown without any zoom (but are not good for scaled image)
            };

            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);

            this.Content = image;
        }

        private void ShowErrorTextBlock(string errorMessage)
        {
            var textBlock = new TextBlock()
            {
                Text = errorMessage,
                Foreground = Brushes.Red,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            this.Content = textBlock;
        }


        private Type GetTypeFromSource(string source)
        {
            Type type;

            // First check if full type name is already written in the source
            type = Type.GetType(source, throwOnError: false);
            if (type == null)
            {
                // If BaseNamespace is
                if (!string.IsNullOrEmpty(BaseNamespace))
                {
                    string fullTypeName = BaseNamespace + "." + source;
                    type = Type.GetType(fullTypeName, throwOnError: false);
                }

                if (type == null)
                {
                    // First try currently executing assembly
                    type = GetTypeFromAssembly(Assembly.GetExecutingAssembly(), source);

                    if (type == null)
                    {
                        // If type is not found, check with entry assembly name
                        type = GetTypeFromAssembly(Assembly.GetEntryAssembly(), source);
                    }
                }
            }

            return type;
        }

        private Type GetTypeFromAssembly(Assembly assembly, string source)
        {
            var usedBaseNamespace = GetAssemblyName(assembly);

            source = usedBaseNamespace + "." + source;

            Type type = Type.GetType(source, throwOnError: false);

            return type;
        }

        private static string GetAssemblyName(Assembly assembly)
        {
            string entryAssemblyFullName = assembly.FullName;
            int pos = entryAssemblyFullName.IndexOf(',');

            string usedBaseNamespace = entryAssemblyFullName.Substring(0, pos);
            return usedBaseNamespace;
        }

        private static string GetResourceName(Assembly assembly, string source)
        {
            var assemblyName = GetAssemblyName(assembly);

            if (!source.StartsWith("/"))
                source = "/" + source;

            return string.Format("pack://application:,,,/{0};component{1}", assemblyName, source);
        }
    }
}