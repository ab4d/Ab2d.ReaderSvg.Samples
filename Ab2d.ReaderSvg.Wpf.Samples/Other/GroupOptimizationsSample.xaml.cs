using System;
using System.Collections.Generic;
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
using Ab2d.Common.ReaderSvg;

namespace Ab2d.Samples.ReaderSvgSamples.Other
{
    /// <summary>
    /// Interaction logic for GroupOptimizationsSample.xaml
    /// </summary>
    public partial class GroupOptimizationsSample : UserControl
    {
        public GroupOptimizationsSample()
        {
            InitializeComponent();

            string svgFileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\groups test.svg");
            ReadSvgFile(svgFileName);
        }

        private void ReadSvgFile(string svgFileName)
        {
            var readerSvg = new Ab2d.ReaderSvg();

            // No optimizations:
            readerSvg.Read(svgFileName);

            var wpfXamlWriterSettings = new WpfXamlWriterSettings()
            {
                IndentStep = 4,
                NumberFormatString = "0.###",
                RootObject = "Canvas",
            };

            NoOptimizationTextBox.Text = readerSvg.GetXaml(wpfXamlWriterSettings);


            // OptimizeObjectGroups
            readerSvg.OptimizeObjectGroups = true;
            readerSvg.Read(svgFileName); // We need to reload the svg file because the OptimizeObjectGroups was changed

            OptimizeObjectGroupsTextBox.Text = readerSvg.GetXaml(wpfXamlWriterSettings);


            // FlattenHierarchies
            readerSvg.FlattenHierarchies = true;
            readerSvg.OptimizeObjectGroups = false;
            readerSvg.Read(svgFileName);

            FlattenHierarchyTextBox.Text = readerSvg.GetXaml(wpfXamlWriterSettings);


            // FlattenHierarchies and TransformShapes
            readerSvg.FlattenHierarchies = true;
            readerSvg.TransformShapes = true;
            readerSvg.Read(svgFileName);

            FlattenAndTransformedHierarchyTextBox.Text = readerSvg.GetXaml(wpfXamlWriterSettings);
        }
    }
}
