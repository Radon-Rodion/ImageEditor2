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
        CanvasController canvasControl;

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
            canvasControl = new CanvasController(canvas);

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
            if (guiControl.SliderChangesTextBox(ContrastSlider, ContrastText)) //message procession needed
            {
                Instrument = -3;
                Rectangle rect = imageControl.GetContrastMaskRect(ContrastSlider.Value);
                canvasControl.CheckAndAdd(rect);
            }
        }

        private void ContrastText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (guiControl == null) return;
            guiControl.TextBoxChangesSlider(ContrastSlider, ContrastText);
        }

        private void BrightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (guiControl == null) return;
            if (guiControl.SliderChangesTextBox(BrightnessSlider, BrightnessText)) //message procession needed
            {
                Instrument = -2;
                Rectangle rect = imageControl.GetBrightnessMaskRect(BrightnessSlider.Value);
                canvasControl.CheckAndAdd(rect);
            }
        }

        private void BrightnessText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (guiControl == null) return;
            guiControl.TextBoxChangesSlider(BrightnessSlider, BrightnessText);
        }

        private void SaturationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (guiControl == null) return;
            if (guiControl.SliderChangesTextBox(SaturationSlider, SaturationText)) //message procession needed
            {
                Instrument = -1;
                Rectangle rect = imageControl.GetSaturationMaskRect(SaturationSlider.Value);
                canvasControl.CheckAndAdd(rect);
            }
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
                    canvasControl.StartPenDrawing(e.GetPosition(canvas));
                    canvasControl.LineToPoint(e.GetPosition(canvas), PenColor.Background, PenWidthSlider.Value);
                    break;
                case 1:     //cropping rectangle
                    canvasControl.StartSelection(e.GetPosition(canvas));
                    break;
            }        
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseClicked) return;
            switch (instrument)
            {
                case 0:     //pen
                    canvasControl.LineToPoint(e.GetPosition(canvas), PenColor.Background, PenWidthSlider.Value);
                    break;
                case 1:     //cropping rectangle
                    canvasControl.ContinueSelection(e.GetPosition(canvas));
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
                    Int32Rect selectedRect = canvasControl.FinishSelection();
                    imageControl.CropImage(selectedRect);
                    instrument = 0;
                    break;
            }
        }

        private void NewFileBtn_Click(object sender, RoutedEventArgs e)
        {
            imageControl.NewImage(BrushColor.Background);
        }

        private void OpenFileBtn_Click(object sender, RoutedEventArgs e)
        {
            Instrument = 0;
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            bool? result = openFileDlg.ShowDialog();
            if (result == true)
            {
                imageControl.OpenImage(openFileDlg.FileName);

            }
            
        }

        private void SaveFileBtn_Click(object sender, RoutedEventArgs e)
        {
            Instrument = 0;
            Microsoft.Win32.SaveFileDialog saveFileDlg = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "BMP Image(*.bmp)|*.bmp|JPG Image(*.jpg)|*.jpg|PNG Image(*.png)|*.png"
            };
            bool? result = saveFileDlg.ShowDialog();
            if (result == true)
            {
                try
                {
                    string fileName = saveFileDlg.FileName;
                    string extension = fileName.Substring(fileName.Length - 3);
                    imageControl.SaveImage(fileName, extension);
                }
                catch (ArgumentException exc)
                {
                    MessageBox.Show(exc.Message);
                }
                
            }
        }

        private void UndoBtn_Click(object sender, RoutedEventArgs e)
        {
            imageControl.Undo();
        }

        private void RedoBtn_Click(object sender, RoutedEventArgs e)
        {
            imageControl.Redo();
        }
    }
}
