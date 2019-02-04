using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace M138ADemo
{
    /// <summary>
    /// Interaction logic for Automatically.xaml
    /// </summary>
    public partial class Automatically : Page
    {
        public Automatically()
        {
            InitializeComponent();
        }

        private void ValidateMessage(object sender, TextCompositionEventArgs eArgs)
        {
            Regex reg = new Regex("[a-zA-Z]+");
            bool valid = reg.IsMatch(eArgs.Text) && ((TextBox)(sender)).Text.Length < 50;
            eArgs.Handled = !valid;
            if (!valid)
            {
                mTextBorder.BorderBrush = Brushes.Red;
                mDangerBlock.Text = "Только латинские буквы";
            }
            else
            {
                mDangerBlock.Text = "";
                mTextBorder.BorderBrush = Brushes.LightSlateGray;
            }
        }
    }
}
