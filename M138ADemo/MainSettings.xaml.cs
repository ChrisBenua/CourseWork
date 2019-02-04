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

namespace M138ADemo
{
    /// <summary>
    /// Interaction logic for MainSettings.xaml
    /// </summary>
    public partial class MainSettings : Window
    {
        public MainSettings()
        {
            InitializeComponent();
            mNextButton.Click += mNextButtonOnClick;
            mMessagetextBox.TextChanged += MMessagetextBox_TextChanged;
            //Configuration.Message = mMessagetextBox.Text;

            mNextButton.IsEnabled = false;
            mNextButton.Background = Brushes.LightGray;
        }

        private void MMessagetextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox box = (TextBox)sender;
            

            box.Text = box.Text.ToUpper().Replace(" ", "");
            box.Text = new String((from ch in box.Text.ToCharArray() where (ch <= 'Z' && ch >= 'A') select ch).ToArray());
            if (box.Text.Length > 100)
            {
                box.Text = box.Text.Substring(0, 100);
            }
            box.CaretIndex = box.Text.Length;

            if (box.Text.Length == 0)
            {
                mNextButton.Background = Brushes.LightGray;
                mNextButton.IsEnabled = false;
            }
            else
            {
                mNextButton.Background = Brushes.Aqua;
                mNextButton.IsEnabled = true;
            }

            //Console.WriteLine(box.Text.Length);
        }

        private void SaveSettings()
        {
            if ((bool)mEncrypt.IsChecked)
            {
                Configuration.Encrypt = true;
            }
            else if ((bool)mDecrypt.IsChecked)
            {
                Configuration.Decrypt = true;
            }

            if ((bool)mAutomaticRadioButton.IsChecked)
            {
                Configuration.Automatic = true;
            }
            else {
                Configuration.Manual = true;
            }

            Configuration.Message = mMessagetextBox.Text;
        }

        private void mNextButtonOnClick(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            if (Configuration.Encrypt)
            {
                AddKeys w = new AddKeys();
                w.Show();
            }
            else
            {
                DecryptAddKeys w = new DecryptAddKeys();
                w.Show();
            }
            this.Close();
        }
    }
}
