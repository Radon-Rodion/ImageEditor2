using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.IO;

namespace SPP_3
{
    static class BitmapMasksCreator
    {
        public static BitmapSource SourceFromBitmap(Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                          source.GetHbitmap(),
                          IntPtr.Zero,
                          Int32Rect.Empty,
                          BitmapSizeOptions.FromEmptyOptions());
        }

        public static Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }

        public static BitmapSource ContrastBitmap(BitmapSource bitmap, float factor)
        {
            Bitmap source = BitmapFromSource(bitmap);
            Bitmap res = ColorMatrixImageChanging(GetContrastMatrix(factor), source);
            return SourceFromBitmap(res);
        }

        public static BitmapSource SaturationBitmap(BitmapSource bitmap, float factor)
        {
            Bitmap source = BitmapFromSource(bitmap);
            Bitmap res = ColorMatrixImageChanging(GetSaturationMatrix(factor), source);
            return SourceFromBitmap(res);
        }

        static float[][] GetContrastMatrix(float cotrastFactor)
        {
            float[][] colorMatrixElements = {
                                                new float[] { 1 / (1- cotrastFactor), 0, 0, 0, 0},
                                                new float[] { 0, 1 / (1 - cotrastFactor), 0, 0, 0},
                                                new float[] { 0, 0, 1 / (1 - cotrastFactor), 0, 0},
                                                new float[] { 0, 0, 0, 1, 0},
                                                new float[] {- 0.75f * cotrastFactor / (1 - cotrastFactor), -0.75f * cotrastFactor / (1 - cotrastFactor), -0.75f *  cotrastFactor / (1 - cotrastFactor), 0, 1}
                                            };
            return colorMatrixElements;
        }

        static float[][] GetSaturationMatrix(float saturationFactor)
        {
            float rwgt = 0.3086f;
            float gwgt = 0.6094f;
            float bwgt = 0.0820f;

            rwgt *= 1 - saturationFactor;
            gwgt *= 1 - saturationFactor;
            bwgt *= 1 - saturationFactor;

            float[][] colorMatrixElements = {
                                                new float[] { rwgt + saturationFactor, rwgt, rwgt, 0, 0},
                                                new float[] { gwgt, gwgt + saturationFactor, gwgt, 0, 0},
                                                new float[] { bwgt, bwgt, bwgt + saturationFactor, 0, 0},
                                                new float[] { 0, 0, 0, 1, 0},
                                                new float[] {0, 0, 0, 0, 1}
                                            };
            return colorMatrixElements;
        }

        static Bitmap ColorMatrixImageChanging(float[][] colorMatrixElements, Bitmap source)
        {

            Bitmap result = new Bitmap(source);
            ImageAttributes imageAttributes = new ImageAttributes();
            int width = result.Width;
            int height = result.Height;

            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(
                colorMatrix,
                ColorMatrixFlag.Default,
                ColorAdjustType.Bitmap);
            Graphics graphics = Graphics.FromImage(result);
            graphics.DrawImage(result, new Rectangle(0, 0, width, height), 0, 0, width,
                               height,
                               GraphicsUnit.Pixel,
                               imageAttributes);

            return result;
        }
    }
}
