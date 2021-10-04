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
using System.Windows.Shapes;

namespace SPP_3
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class ScaleForm : Window
    {
        public ScaleForm()
        {
            InitializeComponent();
        }
        public ScaleForm(int width, int height)
        {
            InitializeComponent();
            NewSize = (width, height);
            WidthBox.Text = $"{width}";
            HeightBox.Text = $"{height}";
        }
        public (int width, int height) NewSize { get; private set; }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.Text = new string
                    (
                         textBox.Text.Where
                         (ch =>
                            Char.IsDigit(ch)
                         )
                         .ToArray()
                    );
            }
        }
        private void ResizeBtn_Click(object sender, RoutedEventArgs e)
        {
            NewSize = (System.Convert.ToInt32(WidthBox.Text), System.Convert.ToInt32(HeightBox.Text));
            this.Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
