using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ReaderSvg.WinFormsSample
{
    public class ImagesHelper
    {
        public static BitmapSource RenderToBitmap(FrameworkElement objectToRender, Brush backgroundBrush)
        {
            if (objectToRender == null)
                return null;

            if (objectToRender.ActualWidth == 0 || double.IsNaN(objectToRender.ActualWidth))
            {
                objectToRender.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                objectToRender.Arrange(new Rect(0, 0, objectToRender.DesiredSize.Width, objectToRender.DesiredSize.Height));
            }

            int objectWidth = Convert.ToInt32(objectToRender.ActualWidth);
            int objectHeight = Convert.ToInt32(objectToRender.ActualHeight);

            var bmp = new RenderTargetBitmap(objectWidth, objectHeight, 96, 96 /* standard WPF dpi setting 96 */, PixelFormats.Pbgra32);

            // Render background
            if (backgroundBrush != null && !ReferenceEquals(backgroundBrush, Brushes.Transparent))
            {
                var backgroundRect = new Rectangle();
                backgroundRect.Width = objectWidth;
                backgroundRect.Height = objectHeight;
                backgroundRect.Fill = backgroundBrush;
                backgroundRect.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                backgroundRect.Arrange(new Rect(0, 0, objectWidth, objectHeight));

                bmp.Render(backgroundRect);
            }
            
            // Render objct
            bmp.Render(objectToRender);

            return bmp;
        }

        public static BitmapSource RenderToBitmap(FrameworkElement objectToRender, int customWidth, int customHeight, int antialiasingLevel, Brush backgroundBrush)
        {
            if (antialiasingLevel > 8)
                throw new ArgumentOutOfRangeException("antialiasingLevel", "antialiasingLevel cannot be bigger than 8");

            if (objectToRender == null)
                return null;


            if (objectToRender.ActualWidth == 0 || double.IsNaN(objectToRender.ActualWidth))
            {
                objectToRender.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                objectToRender.Arrange(new Rect(0, 0, objectToRender.DesiredSize.Width, objectToRender.DesiredSize.Height));
            }

            int objectWidth = Convert.ToInt32(objectToRender.ActualWidth);
            int objectHeight = Convert.ToInt32(objectToRender.ActualHeight);


            // Check if we can do a simple render with using the existing object's size
            if (antialiasingLevel <= 0 &&
                (customWidth <= 0 || customWidth == objectWidth) &&
                (customHeight <= 0 || customHeight == objectHeight))
            {
                return RenderToBitmap(objectToRender, backgroundBrush);
            }


            // Rectangle where the objectToRender will be rendered
            Rect contentRenderRect;

            // Final rectangle
            Rect targetRect;

            if (customWidth > 0 && customHeight > 0)
            {
                double sourceAspect = (double)objectWidth / (double)objectHeight;
                double targetAspect = (double)customWidth / (double)customHeight;

                if (sourceAspect > targetAspect)
                {
                    // source wider than target
                    double newHeight = (targetAspect / sourceAspect) * (double)customHeight;
                    contentRenderRect = new Rect(0, (customHeight - newHeight) / 2, customWidth, newHeight);
                }
                else
                {
                    // source higher than target
                    double newWidth = (sourceAspect / targetAspect) * (double)customWidth;
                    contentRenderRect = new Rect((customWidth - newWidth) / 2, 0, newWidth, customHeight);
                }

                targetRect = new Rect(0, 0, customWidth, customHeight);
            }
            else if (customWidth > 0)
            {
                double newHeight = ((double)customWidth / (double)objectWidth) * (double)objectHeight;
                contentRenderRect = new Rect(0, 0, customWidth, newHeight);
                targetRect = new Rect(0, 0, customWidth, newHeight);
            }
            else // if (customHeight > 0)
            {
                double newWidth = ((double)customHeight / (double)objectHeight) * (double)objectWidth;
                contentRenderRect = new Rect(0, 0, newWidth, customHeight);
                targetRect = new Rect(0, 0, newWidth, customHeight);
            }



            if (antialiasingLevel > 1)
            {
                contentRenderRect = new Rect(contentRenderRect.X * antialiasingLevel,
                                             contentRenderRect.Y * antialiasingLevel,
                                             contentRenderRect.Width * antialiasingLevel,
                                             contentRenderRect.Height * antialiasingLevel);

                targetRect = new Rect(0, 0, targetRect.Width * antialiasingLevel, targetRect.Height * antialiasingLevel);
            }


            var drawingVisual = new DrawingVisual();

            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                if (backgroundBrush != null && !ReferenceEquals(backgroundBrush, Brushes.Transparent))
                    drawingContext.DrawRectangle(backgroundBrush, null, targetRect);

                var visualBrush = new VisualBrush(objectToRender);
                drawingContext.DrawRectangle(visualBrush, null, contentRenderRect);
            }


            var renderTargetBitmap = new RenderTargetBitmap((int)targetRect.Width, (int)targetRect.Height, 96.0, 96.0, PixelFormats.Default);
            renderTargetBitmap.Render(drawingVisual);



            BitmapSource finalBitmap;

            if (antialiasingLevel > 1)
            {
                var downScale = new ScaleTransform(1.0 / (double)antialiasingLevel, 1.0 / (double)antialiasingLevel);
                finalBitmap = new TransformedBitmap(renderTargetBitmap, downScale);
            }
            else
            {
                finalBitmap = renderTargetBitmap;
            }

            return finalBitmap;
        }        
    }
}