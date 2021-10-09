using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Drawing.Imaging;
using SPP_3;
using System.Windows;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        const string TESTING_FILES_LOCATION = "TestImages\\";
        [TestMethod]
        public void TestOpeningAndSavingBmp()
        {
            Bitmap ethalonBitmap = new Bitmap(Image.FromFile(TESTING_FILES_LOCATION + "basicImage.bmp"));
            ImageController controller = new ImageController();
            controller.OpenImage(TESTING_FILES_LOCATION + "basicImage.bmp");
            controller.SaveImage(TESTING_FILES_LOCATION + "basicImageSave.bmp", "bmp");
            Bitmap savedBitmap = new Bitmap(Image.FromFile(TESTING_FILES_LOCATION + "basicImageSave.bmp"));
            Assert.IsTrue(BitmapsEquals(ethalonBitmap, savedBitmap), "Invalid bmp saving!");
        }

        [TestMethod]
        public void TestOpeningAndSavingPng()
        {
            Bitmap ethalonBitmap = new Bitmap(Image.FromFile(TESTING_FILES_LOCATION + "basicImage.png"));
            ImageController controller = new ImageController();
            controller.OpenImage(TESTING_FILES_LOCATION + "basicImage.png");
            controller.SaveImage(TESTING_FILES_LOCATION + "basicImageSave.png", "png");
            Bitmap savedBitmap = new Bitmap(Image.FromFile(TESTING_FILES_LOCATION + "basicImageSave.png"));
            Assert.IsTrue(BitmapsEquals(ethalonBitmap, savedBitmap), "Invalid png saving!");
        }

        [TestMethod]
        public void TestRotation()
        {
            Bitmap ethalonBitmap1 = new Bitmap(Image.FromFile(TESTING_FILES_LOCATION + "basicImage90Right.bmp"));
            Bitmap ethalonBitmap2 = new Bitmap(Image.FromFile(TESTING_FILES_LOCATION + "basicImage90Left.bmp"));
            ImageController controller = new ImageController();
            controller.OpenImage(TESTING_FILES_LOCATION + "basicImage.bmp");
            controller.Rotate(90);
            controller.SaveImage(TESTING_FILES_LOCATION + "basicImageSave90Right.bmp", "bmp");
            controller.Rotate(-180f);
            controller.SaveImage(TESTING_FILES_LOCATION + "basicImageSave90Left.bmp", "bmp");
            Bitmap savedBitmap1 = new Bitmap(Image.FromFile(TESTING_FILES_LOCATION + "basicImageSave90Right.bmp"));
            Bitmap savedBitmap2 = new Bitmap(Image.FromFile(TESTING_FILES_LOCATION + "basicImageSave90Left.bmp"));
            Assert.IsTrue(BitmapsEquals(ethalonBitmap1, savedBitmap1), "Rotation 90 degrees right is incorrect!");
            Assert.IsTrue(BitmapsEquals(ethalonBitmap2, savedBitmap2), "Rotation 180 degrees left after 90 degrees right is incorrect!");
        }

        [TestMethod]
        public void TestReflection()
        {
            Bitmap ethalonBitmap1 = new Bitmap(Image.FromFile(TESTING_FILES_LOCATION + "basicImageReflectHor.bmp"));
            Bitmap ethalonBitmap2 = new Bitmap(Image.FromFile(TESTING_FILES_LOCATION + "basicImageReflectCenter.bmp"));
            ImageController controller = new ImageController();
            controller.OpenImage(TESTING_FILES_LOCATION + "basicImage.bmp");
            controller.ScaleImage(-1, 1);
            controller.SaveImage(TESTING_FILES_LOCATION + "basicImageSaveReflectHor.bmp", "bmp");
            controller.ScaleImage(1, -1);
            controller.SaveImage(TESTING_FILES_LOCATION + "basicImageSaveReflectCenter.bmp", "bmp");
            Bitmap savedBitmap1 = new Bitmap(Image.FromFile(TESTING_FILES_LOCATION + "basicImageSaveReflectHor.bmp"));
            Bitmap savedBitmap2 = new Bitmap(Image.FromFile(TESTING_FILES_LOCATION + "basicImageSaveReflectCenter.bmp"));
            Assert.IsTrue(BitmapsEquals(ethalonBitmap1, savedBitmap1), "Horizontal reflection is incorrect!");
            Assert.IsTrue(BitmapsEquals(ethalonBitmap2, savedBitmap2), "Verical reflection after horizontal is incorrect!");
        }

        [TestMethod]
        public void TestScaling()
        {
            Bitmap ethalonBitmap1 = new Bitmap(Image.FromFile(TESTING_FILES_LOCATION + "basicImageScale(1,5x0,5).bmp"));
            Bitmap ethalonBitmap2 = new Bitmap(Image.FromFile(TESTING_FILES_LOCATION + "basicImageScale(3x1,75).bmp"));
            ImageController controller = new ImageController();
            controller.OpenImage(TESTING_FILES_LOCATION + "basicImage.bmp");
            controller.ScaleImage(1.5f, 0.5f);
            controller.SaveImage(TESTING_FILES_LOCATION + "basicImageSaveScale(1,5x0,5).bmp", "bmp");
            controller.ScaleImage(2f, 3.5f);
            controller.SaveImage(TESTING_FILES_LOCATION + "basicImageSaveScale(3x1,75).bmp", "bmp");
            Bitmap savedBitmap1 = new Bitmap(Image.FromFile(TESTING_FILES_LOCATION + "basicImageSaveScale(1,5x0,5).bmp"));
            Bitmap savedBitmap2 = new Bitmap(Image.FromFile(TESTING_FILES_LOCATION + "basicImageSaveScale(3x1,75).bmp"));
            Assert.IsTrue(BitmapsEquals(ethalonBitmap1, savedBitmap1), "Scaling 1,5x0,5 is incorrect!");
            Assert.IsTrue(BitmapsEquals(ethalonBitmap2, savedBitmap2), "Scaling 2x3,5 after 1,5x0,5 is incorrect!");
        }

        [TestMethod]
        public void TestCropping()
        {
            Bitmap ethalonBitmap1 = new Bitmap(Image.FromFile(TESTING_FILES_LOCATION + "basicImageResize(200x200).bmp"));
            Bitmap ethalonBitmap2 = new Bitmap(Image.FromFile(TESTING_FILES_LOCATION + "newBlackImageResize(500x400).bmp"));
            ImageController controller = new ImageController();
            controller.OpenImage(TESTING_FILES_LOCATION + "basicImage.bmp");
            controller.CropImage(new Int32Rect(0, 0, 200, 200));
            controller.SaveImage(TESTING_FILES_LOCATION + "basicImageSaveResize(200x200).bmp", "bmp");
            controller.NewImage(System.Windows.Media.Brushes.Black);
            controller.CropImage(new Int32Rect(0, 0, 500, 400));
            controller.SaveImage(TESTING_FILES_LOCATION + "newImageSaveResize(500x400).bmp", "bmp");
            Bitmap savedBitmap1 = new Bitmap(Image.FromFile(TESTING_FILES_LOCATION + "basicImageSaveResize(200x200).bmp"));
            Bitmap savedBitmap2 = new Bitmap(Image.FromFile(TESTING_FILES_LOCATION + "newImageSaveResize(500x400).bmp"));
            Assert.IsTrue(BitmapsEquals(ethalonBitmap1, savedBitmap1), "Resizing 200x200 is incorrect!");
            Assert.IsTrue(BitmapsEquals(ethalonBitmap2, savedBitmap2), "Resizing 500x400 after creating new blue image is incorrect!");
        }

        unsafe private bool BitmapsEquals(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1.Size != bmp2.Size)
                return false;
            BitmapData bmpd1 = bmp1.LockBits(new Rectangle(0, 0, bmp1.Width, bmp1.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            BitmapData bmpd2 = bmp2.LockBits(new Rectangle(0, 0, bmp2.Width, bmp2.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Boolean res = true;
            Int32* p1 = (Int32*)bmpd1.Scan0;
            Int32* p2 = (Int32*)bmpd2.Scan0;
            for (Int32 i = 0; i < bmpd1.Height; i++)
            {
                for (Int32 j = 0; j < bmpd2.Width; j++)
                {
                    if (*p1 != *p2)
                    {
                        res = false;
                        break;
                    }
                    p1++;
                    p2++;
                }
            }
            bmp1.UnlockBits(bmpd1);
            bmp2.UnlockBits(bmpd2);
            return res;
        }
    }
}
