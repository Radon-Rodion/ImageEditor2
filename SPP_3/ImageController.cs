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
using System.IO;

namespace SPP_3
{
    public class ImageController
    {
        Image picture;
        WriteableBitmap bitmap;
        Stack<BitmapSource> undo;
        Stack<BitmapSource> redo;
        //int ind;

        public ImageController(Image picture = null)
        {
            this.picture = picture;
            undo = new Stack<BitmapSource>();
            redo = new Stack<BitmapSource>();
            //ind = 0;
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
            TransformedBitmap tb = new TransformedBitmap();

            tb.BeginInit();
            tb.Source = bitmap;
            tb.Transform = transform;
            tb.EndInit();

            UpdateBitmap(tb);
        }

        public void OpenImage(string address)
        {
            //Open Image
            BitmapImage bitmapImage = new BitmapImage();

            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(address);
            //bitmapImage.DecodePixelWidth = 750;
            bitmapImage.EndInit();

            UpdateBitmap(bitmapImage);
        }

        public void SaveImage(string address, string format)
        {
            BitmapEncoder encoder;
            switch (format)
            {
                case "bmp":
                    encoder = new BmpBitmapEncoder();
                    break;
                case "jpg":
                    encoder = new JpegBitmapEncoder();
                    break;
                case "png":
                    encoder = new PngBitmapEncoder();
                    break;
                default:
                    throw new ArgumentException($"Invalid format: {format}");
            }
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using (FileStream fileStream = new FileStream(address, FileMode.Create))
                encoder.Save(fileStream);
        }

        public void NewImage(Brush background)
        {
            Rectangle rect = new Rectangle();
            rect.Width = 750;
            rect.Height = 500;
            rect.Fill = background;
            RenderTargetBitmap renderBmp = new RenderTargetBitmap(750, 500, 96d, 96d, PixelFormats.Pbgra32);
            renderBmp.Render(rect);
            UpdateBitmap(renderBmp);
        }

        public void CropImage(Int32Rect cropRect)
        {
            CroppedBitmap cropBmp = new CroppedBitmap(bitmap, cropRect);
            UpdateBitmap(cropBmp);
        }

        public void Undo()
        {
            try
            {
                if (undo.Peek() == null) return;
                BitmapSource bmp = undo.Pop();
                redo.Push(bitmap);
                UpdateWithoutStack(bmp);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Nothing to undo");
            }
        }

        public void Redo()
        {
            try
            {
                if (redo.Peek() == null) return;
                BitmapSource bmp = redo.Pop();
                undo.Push(bitmap);
                UpdateWithoutStack(bmp);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Nothing to redo");
            }
        }

        private void UpdateWithoutStack(BitmapSource bmpSource)
        {
            bitmap = new WriteableBitmap(bmpSource);
            if (picture != null)
                picture.Source = bitmap;
        }

        private void UpdateBitmap(BitmapSource bmpSource)
        {
            undo.Push(bitmap);
            redo.Clear();
            UpdateWithoutStack(bmpSource);
            //ind++;
            //SaveImage($"G:/tempor/tempImg{ind}.bmp");
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
