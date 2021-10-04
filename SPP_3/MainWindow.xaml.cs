using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SPP_3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        GUIController guiControl;
        ImageController imageControl;

        bool mouseClicked;
        int instrument;
        int Instrument
        {
            set
            {
                if (instrument != value)
                {
                    instrument = value;
                    imageControl.UpdateFromCanvas(canvas);
                    guiControl.SlidersToZero();
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            mouseClicked = false;
            instrument = 0; //pen

            guiControl = new GUIController(PenColor, (ContrastSlider, BrightnessSlider, SaturationSlider));
            imageControl = new ImageController(picture);

            foreach (UIElement el in Colors.Children)
            {
                if(el is Button)
                {
                    ((Button)el).Click += ColorButton_Click;
                }
            }
        }

        private void ContrastSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (guiControl == null) return;
            guiControl.SliderChangesTextBox(ContrastSlider, ContrastText);

            Instrument = -3;
            Rectangle rect = imageControl.GetContrastMaskRect(ContrastSlider.Value);
            if (!canvas.Children.Contains(rect)) canvas.Children.Add(rect);
        }

        private void ContrastText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (guiControl == null) return;
            guiControl.TextBoxChangesSlider(ContrastSlider, ContrastText);
        }

        private void BrightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (guiControl == null) return;
            guiControl.SliderChangesTextBox(BrightnessSlider, BrightnessText);

            Instrument = -2;
            Rectangle rect = imageControl.GetBrightnessMaskRect(BrightnessSlider.Value);
            if(!canvas.Children.Contains(rect)) canvas.Children.Add(rect);
        }

        private void BrightnessText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (guiControl == null) return;
            guiControl.TextBoxChangesSlider(BrightnessSlider, BrightnessText);
        }

        private void SaturationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (guiControl == null) return;
            guiControl.SliderChangesTextBox(SaturationSlider, SaturationText);

            Instrument = -1;
            Rectangle rect = imageControl.GetSaturationMaskRect(SaturationSlider.Value);
            if (!canvas.Children.Contains(rect)) canvas.Children.Add(rect);
        }

        private void SaturationText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (guiControl == null) return;
            guiControl.TextBoxChangesSlider(SaturationSlider, SaturationText);
        }

        private void PenWidthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (guiControl == null) return;
            guiControl.SliderChangesTextBox(PenWidthSlider, PenWidthText);
            /*controller.SetPenWidth(lineWidthTrack.Value);*/
        }

        private void PenWidthText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (guiControl == null) return;
            guiControl.TextBoxChangesSlider(PenWidthSlider, PenWidthText);
        }

        private void ColorControl_Click(object sender, RoutedEventArgs e)
        {
            if (guiControl == null) return;
            guiControl.SetColorControl((Button)e.OriginalSource);
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (guiControl == null) return;
            guiControl.ChangeColor(((Button)e.OriginalSource).Background);
        }

        private void OpenFileBtn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            bool? result = openFileDlg.ShowDialog();
            if (result == true)
            {
                imageControl.OpenImage(openFileDlg.FileName);
               
            }
            Instrument = 0;
        }

        private void RotateLeftButton_Click(object sender, RoutedEventArgs e)
        {
            Instrument = 0;
            imageControl.Rotate(-90);
        }

        private void RotateRightButton_Click(object sender, RoutedEventArgs e)
        {
            Instrument = 0;
            imageControl.Rotate(90);
        }

        private void ReflectHorButton_Click(object sender, RoutedEventArgs e)
        {
            Instrument = 0;
            imageControl.ScaleImage(-1, 1);
        }

        private void ReflectVertButton_Click(object sender, RoutedEventArgs e)
        {
            Instrument = 0;
            imageControl.ScaleImage(1, -1);
        }

        private void ResizeButton_Click(object sender, RoutedEventArgs e)
        {
            Instrument = 0;
            imageControl.ResizeImage();
        }

        private void ClipButton_Click(object sender, RoutedEventArgs e)
        {
            Instrument = 1;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (instrument < 0) Instrument = 0;

            mouseClicked = true;
            switch (instrument)
            {
                case 0:     //pen
                    PenDrawer drawer = new PenDrawer(e.GetPosition(canvas));
                    Line line = drawer.LineTo(e.GetPosition(canvas));
                    line.Stroke = PenColor.Background;
                    line.Fill = PenColor.Background;
                    line.StrokeThickness = PenWidthSlider.Value;
                    canvas.Children.Add(line);
                    break;
                case 1:     //cropping rectangle
                    SelectionRect selRect = new SelectionRect(e.GetPosition(canvas));
                    canvas.Children.Add(selRect.GetRectFigure());
                    break;
            }        
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseClicked) return;
            switch (instrument)
            {
                case 0:     //pen
                    PenDrawer drawer = PenDrawer.GetPenDrawer();
                    Line line = drawer.LineTo(e.GetPosition(canvas));
                    line.Stroke = PenColor.Background;
                    line.Fill = PenColor.Background;
                    line.StrokeThickness = PenWidthSlider.Value;
                    canvas.Children.Add(line);
                    //imageControl.DrawPoint((int)e.GetPosition(canvas).X, (int)e.GetPosition(canvas).Y);
                    break;
                case 1:     //cropping rectangle
                    SelectionRect selRect = SelectionRect.GetSelectionRect();
                    selRect.SetSecondPoint(e.GetPosition(canvas));
                    break;
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mouseClicked = false;
            switch (instrument)
            {
                case 0:     //pen
                    imageControl.UpdateFromCanvas(canvas);
                    break;
                case 1:     //cropping rectangle
                    SelectionRect selRect = SelectionRect.GetSelectionRect();
                    canvas.Children.Remove(selRect.GetRectFigure());
                    imageControl.CropImage(selRect.GetSelectionArea());
                    Instrument = 0;
                    break;
            }
        }

        private class SelectionRect
        {
            private static SelectionRect selectionRect;
            private Rectangle rect;
            double xFirst, yFirst;

            public SelectionRect(Point firstPoint)
            {
                this.xFirst = firstPoint.X;
                this.yFirst = firstPoint.Y;

                rect = new Rectangle();
                ChangeRectangle(xFirst, yFirst, 0, 0);
                rect.Stroke = Brushes.Black;
                rect.StrokeThickness = 3;

                selectionRect = this;
            }
            public static SelectionRect GetSelectionRect()
            {
                return selectionRect;
            }

            public void SetSecondPoint(Point secondPoint)
            {
                double xLeft = secondPoint.X > xFirst ? xFirst : secondPoint.X;
                double yTop = secondPoint.Y > yFirst ? yFirst : secondPoint.Y;

                double width = Math.Abs(secondPoint.X - xFirst);
                double height = Math.Abs(secondPoint.Y - yFirst);
                ChangeRectangle(xLeft, yTop, width, height);
            }

            private void ChangeRectangle(double xLeft, double yTop, double width, double height)
            {
                Canvas.SetLeft(rect, xLeft);
                Canvas.SetTop(rect, yTop);
                rect.Width = width;
                rect.Height = height;
            }

            public Rectangle GetRectFigure()
            {
                return rect;
            }

            public Int32Rect GetSelectionArea()
            {
                int x = System.Convert.ToInt32(Canvas.GetLeft(rect));
                int y = System.Convert.ToInt32(Canvas.GetTop(rect));
                int width = System.Convert.ToInt32(rect.Width);
                int height = System.Convert.ToInt32(rect.Height);

                Int32Rect area = new Int32Rect(x, y, width, height);
                return area;
            }
        }

        private class PenDrawer
        {
            static PenDrawer drawer;
            Point prevPoint;

            public PenDrawer(Point point)
            {
                prevPoint = point;
                drawer = this;
            }

            public static PenDrawer GetPenDrawer()
            {
                return drawer;
            }

            public Line LineTo(Point nextPoint)
            {
                Line line = new Line();
                line.X1 = prevPoint.X;
                line.Y1 = prevPoint.Y;
                line.X2 = nextPoint.X;
                line.Y2 = nextPoint.Y;

                prevPoint = nextPoint;
                return line;
            }
        }
    }
}
