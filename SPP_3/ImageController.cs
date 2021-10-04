using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SPP_3
{
    class ImageController
    {
        Image picture;
        WriteableBitmap bitmap;

        public ImageController(Image picture)
        {
            this.picture = picture;
        }

        public void Rotate(double angle)
        {
            TransformImage(new RotateTransform(angle));
        }

        public void ScaleImage(double scaleX, double scaleY)
        {
            TransformImage(new ScaleTransform(scaleX, scaleY));
        }

        public void ResizeImage()
        {
            ScaleForm scaleForm = new ScaleForm(System.Convert.ToInt32(bitmap.Width), System.Convert.ToInt32(bitmap.Height));
            scaleForm.ShowDialog();
            (int width, int height) newSize = scaleForm.NewSize;

            double scaleX = newSize.width / bitmap.Width;
            double scaleY = newSize.height / bitmap.Height;
            ScaleImage(scaleX, scaleY);
        }

        private void TransformImage(Transform transform)
        {
            // Create the TransformedBitmap to use as the Image source.
            TransformedBitmap tb = new TransformedBitmap();

            // Properties must be set between BeginInit and EndInit calls.
            tb.BeginInit();
            tb.Source = bitmap;
            // Set image transformatrion.
            tb.Transform = transform;
            tb.EndInit();

            // Set the Image source.
            UpdateBitmap(tb);
        }

        public void OpenImage(string address)
        {
            //Open Image
            BitmapImage bitmapImage = new BitmapImage();

            // BitmapImage.UriSource must be in a BeginInit/EndInit block
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(address);

            // To save significant application memory, set the DecodePixelWidth or
            // DecodePixelHeight of the BitmapImage value of the image source to the desired
            // height or width of the rendered image. If you don't do this, the application will
            // cache the image as though it were rendered as its normal size rather then just
            // the size that is displayed.
            // Note: In order to preserve aspect ratio, set DecodePixelWidth
            // or DecodePixelHeight but not both.
            //myBitmapImage.DecodePixelWidth = 200;
            bitmapImage.EndInit();

            UpdateBitmap(bitmapImage);
        }

        public void DrawPoint(int x, int y)
        {

            try
            {
                // Reserve the back buffer for updates.
                bitmap.Lock();

                unsafe
                {
                    // Get a pointer to the back buffer.
                    IntPtr pBackBuffer = bitmap.BackBuffer;

                    // Find the address of the pixel to draw.
                    pBackBuffer += y * bitmap.BackBufferStride;
                    pBackBuffer += x * 4;

                    // Compute the pixel's color.
                    /*int color_data = 255 << 16; // R
                    color_data |= 128 << 8;   // G
                    color_data |= 255 << 0;   // B*/

                    // Assign the color data to the pixel.
                    *((int*)pBackBuffer) = 0;//color_data;
                }

                // Specify the area of the bitmap that changed.
                bitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
            }
            finally
            {
                // Release the back buffer and make it available for display.
                bitmap.Unlock();
            }
        }

        public void CropImage(Int32Rect cropRect)
        {
            CroppedBitmap cropBmp = new CroppedBitmap(bitmap, cropRect);
            UpdateBitmap(cropBmp);
        }

        public void UpdateBitmap(BitmapSource bmpSource)
        {
            bitmap = new WriteableBitmap(bmpSource);
            picture.Source = bitmap;
        }

        public void UpdateFromCanvas(Canvas canvas)
        {
            // Рендерим канвас в битмап
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)bitmap.Width, (int)bitmap.Height, 96d, 96d, PixelFormats.Pbgra32);
            renderBitmap.Render(canvas);

            UpdateBitmap(renderBitmap);
            canvas.Children.Clear();
            canvas.Children.Add(picture);

            MaskFactory.DropRect(); //in order to definitely have new mask rectangle next time
        }

        public FormatConvertedBitmap GetSaturationMask(double decVal)
        {
            FormatConvertedBitmap formatBitmap = new FormatConvertedBitmap(bitmap, PixelFormats.Gray16, BitmapPalettes.BlackAndWhite, decVal);
            return formatBitmap;
        }

        public Rectangle GetBrightnessMaskRect(double brightnessFactor)
        {
            return MaskFactory.GetMaskRect(2, bitmap, brightnessFactor);
        }

        public Rectangle GetSaturationMaskRect(double saturationFactor)
        {
            return MaskFactory.GetMaskRect(1, bitmap, saturationFactor);
        }

        public Rectangle GetContrastMaskRect(double contrastFactor)
        {
            return MaskFactory.GetMaskRect(3, bitmap, contrastFactor);
        }

        private class MaskFactory { 
            static Rectangle rect;
            static int type;
            static Brush minFactorBrush, maxFactorBrush;
            public static void DropRect()
            {
                rect = null;
                type = 0;
            }
            public static Rectangle GetMaskRect(int type, in BitmapSource bitmap, double brushFactor)
            {
                if (type != MaskFactory.type && rect == null)
                {
                    ChangeBrushes(type, bitmap);
                    MaskFactory.type = type;
                    rect = new Rectangle();
                    rect.Width = bitmap.Width;
                    rect.Height = bitmap.Height;
                    Canvas.SetLeft(rect, 0);
                    Canvas.SetTop(rect, 0);
                    Canvas.SetZIndex(rect, 1);
                }

                BrushRect(brushFactor);
                return rect;
            }

            static void BrushRect(double factor)
            {
                if (factor < 0)
                {
                    rect.Fill = minFactorBrush;
                    rect.Opacity = -factor / 128;
                } else
                {
                    rect.Fill = maxFactorBrush;
                    rect.Opacity = factor / 128;
                }
            }

            static void ChangeBrushes(int type, BitmapSource bitmap)
            {
                switch (type)
                {
                    case 1: //Saturation
                        minFactorBrush = new ImageBrush(
                            new FormatConvertedBitmap(bitmap, PixelFormats.Gray16, BitmapPalettes.BlackAndWhite, 1)
                            );
                        maxFactorBrush = new ImageBrush(BitmapMasksCreator.SaturationBitmap(bitmap, 3));
                        break;
                    case 2: //Brightness
                        minFactorBrush = new SolidColorBrush(Colors.Black);
                        maxFactorBrush = new SolidColorBrush(Colors.White);
                        break;
                    case 3: //Contrast
                        minFactorBrush = new ImageBrush(BitmapMasksCreator.ContrastBitmap(bitmap, -1));
                        maxFactorBrush = new ImageBrush(BitmapMasksCreator.ContrastBitmap(bitmap, 0.9f));
                        break;
                }
            }
        }

        
    }
}
