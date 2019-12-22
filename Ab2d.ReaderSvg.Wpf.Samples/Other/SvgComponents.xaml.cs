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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Effects;

namespace Ab2d.Samples.ReaderSvgSamples.Other
{
    /// <summary>
    /// Interaction logic for SvgComponents.xaml
    /// </summary>

    public partial class SvgComponents : UserControl
    {
        Ab2d.ReaderSvg myReaderSvg;
        FrameworkElement selectedElement;
        DragAndDropHelper _dragAndDropHelper;

        public SvgComponents()
        {
            InitializeComponent();

            _dragAndDropHelper = new DragAndDropHelper(this, ".svg;.gsvg");
            _dragAndDropHelper.FileDroped += new EventHandler<FileDropedEventArgs>(dragAndDropHelper_FileDroped);
        }

        public void PageLoaded(object sender, RoutedEventArgs e)
        {
            LoadSvg(AppDomain.CurrentDomain.BaseDirectory + @"Resources\svg_layout.svg");
        }

        void dragAndDropHelper_FileDroped(object sender, FileDropedEventArgs e)
        {
            LoadSvg(e.FileName);
        }

        public void LoadOnClick(object sender, RoutedEventArgs e)
        {
            ShowOpenFileDialog();
        }

        private void ShowOpenFileDialog()
        {
            string fileName = OpenSvgFileHelper.ShowOpenFileDialog();
            if (fileName != null)
                LoadSvg(fileName);
        }

        void SvgIdsListBoxSelected(object sender, SelectionChangedEventArgs args)
        {
            string selectedId;

            selectedId = ((sender as ListBox).SelectedItem as string);

            if (selectedId == null) return;

            // .Net 3.5 SP1 compatable code
            if (selectedElement != null)
                selectedElement.Effect = null;

            DropShadowEffect dropShadowEffect = new DropShadowEffect();
            dropShadowEffect.Color = Colors.Red;
            dropShadowEffect.ShadowDepth = 0; // this makes it appear as outer glow
            dropShadowEffect.BlurRadius = 15;
            dropShadowEffect.Opacity = 0.95;

            selectedElement = myReaderSvg.NamedObjects[selectedId] as FrameworkElement;

            if (selectedElement != null)
                selectedElement.Effect = dropShadowEffect;


            // OLD .Net 3.0 compatable code:
            //if (selectedElement != null)
            //    selectedElement.BitmapEffect = null;

            //OuterGlowBitmapEffect outerGlowEffect = new OuterGlowBitmapEffect();
            //outerGlowEffect.GlowColor = Colors.Red;
            //outerGlowEffect.GlowSize = 6;
            //outerGlowEffect.Noise = 0;
            //outerGlowEffect.Opacity = 0.8;

            //selectedElement = myReaderSvg.NamedObjects[selectedId] as FrameworkElement;

            //if (selectedElement != null)
            //    selectedElement.BitmapEffect = outerGlowEffect;
        }

        private void LoadSvg(string fileName)
        {
            myReaderSvg = new Ab2d.ReaderSvg();

            Viewbox importedElement;

            importedElement = myReaderSvg.Read(fileName);

            SvgGrid.Children.Clear();
            SvgGrid.Children.Add(importedElement);

            SvgIdsListBox.Items.Clear();

            if (myReaderSvg.NamedObjects.Count == 0)
            {
                MessageBox.Show("This SVG file does not contain elements with id set!");
            }
            else
            {
                foreach (string key in myReaderSvg.NamedObjects.Keys)
                {
                    SvgIdsListBox.Items.Add(key);
                }
            }
        }

    }
}