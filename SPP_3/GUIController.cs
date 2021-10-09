using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SPP_3
{
    class GUIController
    {
        (Slider contrastSlider, Slider brightnessSlider, Slider saturationSlider) sliders;
        Button penColorControl;
        Button colorController;
        bool needMessageProcession;

        public GUIController(Button penColorControl, (Slider contrastSlider, Slider brightnessSlider, Slider saturationSlider) sliders)
        {
            this.colorController = this.penColorControl = penColorControl;
            this.sliders = sliders;
            needMessageProcession = true;
        }

        public bool SliderChangesTextBox(Slider slider, TextBox box)
        {
            box.Text = $"{System.Convert.ToInt32(slider.Value)}";
            return needMessageProcession;
        }

        public void TextBoxChangesSlider(Slider slider, TextBox box)
        {
            try
            {
                if (!box.Text.Equals(""))
                    slider.Value = System.Convert.ToInt32(box.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid value!");
            }
        }

        /// <summary>
        /// Changes color of the colorController button and returns true if changing pen color.
        /// </summary>
        public bool ChangeColor(Brush newColor) //true if changing pen color; false - if brush color
        {
            if (colorController != null)
            {
                colorController.Background = newColor;
                if (colorController == penColorControl) return true;
            }
            return false;
        }

        public void SlidersToZero()
        {
            needMessageProcession = false;
            sliders.contrastSlider.Value = 0;
            sliders.brightnessSlider.Value = 0;
            sliders.saturationSlider.Value = 0;
            needMessageProcession = true;
        }

        public void SetColorControl(Button newColorControl)
        {
            if (newColorControl != colorController)
            {
                /*colorController.FlatStyle = FlatStyle.Popup;
                newColorControl.FlatStyle = FlatStyle.Standard;*/
                colorController = newColorControl;
            }
        }
    }
}
