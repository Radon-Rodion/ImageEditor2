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
        public MainWindow()
        {
            InitializeComponent();
            guiControl = new GUIController(PenColor, (ContrastSlider, BrightnessSlider, SaturationSlider));
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
            /*Instrument = -1;
            guiControl.ChangeChromaticity((chromaticityTrack.Value / 128f) + 1);*/
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
            /*Instrument = -2;
            guiControl.ChangeChromaticity((chromaticityTrack.Value / 128f) + 1);*/
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
            /*Instrument = -3;
            guiControl.ChangeChromaticity((chromaticityTrack.Value / 128f) + 1);*/
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
    }
}
