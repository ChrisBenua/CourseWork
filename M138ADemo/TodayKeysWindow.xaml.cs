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
    /// Interaction logic for TodayKeysWindow.xaml
    /// </summary>
    public partial class TodayKeysWindow : Window
    {
        public TodayKeysWindow()
        {
            InitializeComponent();
            generateButton.Click += GenerateButtonOnClick;
        }

        private void GenerateButtonOnClick(object sender, RoutedEventArgs e)
        {
            bool success;
            string messageBoxText = "";
            string messageBoxCaption = "";
            int res = 0;
            (success, messageBoxText, messageBoxCaption, res) = ValidateTextFieldText();
            var result = MessageBox.Show(messageBoxText, messageBoxCaption, MessageBoxButton.OK);
            if (result == MessageBoxResult.OK)
            {
                if (messageBoxCaption[0] == 'У')
                {
                    Configuration.lst.Clear();
                    DateTime time = DateTime.Today;
                    Configuration.lst.Clear();
                    foreach (var el in Generator.GenerateRandomKeys(res, time.DayOfYear + 366 * time.Year))
                    {
                        Configuration.lst.Add(el);
                    }
                    //Configuration.lst = Generator.GenerateRandomKeys(res, time.DayOfYear + 366 * time.Year);
                    Close();
                }
            }
        }

        private (bool, String, String, int) ValidateTextFieldText()
        {
            string text = numberOfKeysTextBlock.Text;
            int res = 0;
            string messageBoxText = "";
            string messageBoxCaption = "";
            bool success = false;
            if (int.TryParse(text, out res) && res > 0 && res <= Helper.MaxKeys)
            {
                Configuration.lst.Clear();
               
                messageBoxText = "Ключи были сгенерированы успешно";
                messageBoxCaption = "Успех";
                success = true;
            }
            else
            {
                messageBoxText = "Ошибка, проверьте поле для ввода количества ключей";
                messageBoxCaption = "Ошибка";
            }

            return (success, messageBoxText, messageBoxCaption, res);

        }
    }
}
