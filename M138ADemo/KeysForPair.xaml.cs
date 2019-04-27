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
    /// Interaction logic for KeysForPair.xaml
    /// </summary>
    public partial class KeysForPair : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.KeysForPair"/> class.
        /// </summary>
        public KeysForPair()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The current pair key.
        /// </summary>
        public int CurrentPairKey = 0;

        /// <summary>
        /// Keies the box on preview text input.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void KeyBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
            if (Helper.reg.IsMatch(e.Text))
            {
                if (((TextBox)(sender)).Text.Length <= 7)
                {
                    CurrentPairKey = int.Parse(((TextBox) (sender)).Text + e.Text);
                    e.Handled = false;
                }
                else
                {
                    MessageBox.Show("Ошибка", "Слишком большое число", MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBox.Show("Ошибка", "Только цифры, пожалуйста", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Amounts the of keys on preview text input.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void AmountOfKeys_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
            if (Helper.reg.IsMatch(e.Text))
            {
                if (((TextBox)(sender)).Text.Length <= 7)
                {
                    int cnt = int.Parse(((TextBox) (sender)).Text + e.Text);
                    if (cnt <= 100)
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        e.Handled = true;
                        MessageBox.Show("Ошибка", "Слишком большое число", MessageBoxButton.OK);
                    }
                    
                }
                else
                {
                    MessageBox.Show("Ошибка", "Слишком большое число", MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBox.Show("Ошибка", "Только цифры, пожалуйста", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Chooses the button on click.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void ChooseButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (KeyBox.Text.Length > 0 && AmountOfKeys.Text.Length > 0 && Helper.reg.IsMatch(KeyBox.Text) &&
                Helper.reg.IsMatch(AmountOfKeys.Text))
            {
                int n = (int.Parse(AmountOfKeys.Text));
                Configuration.KeyList.Clear();
                var kek = Generator.GenerateRandomKeys(n, Int32.Parse(KeyBox.Text));

                for (int i = 0; i < n; ++i)
                {
                    Configuration.KeyList.Add(kek[i]);
                }
                MessageBox.Show("Успешно", "Ключи были сгенерированы успешно", MessageBoxButton.OK);
                this.Close();
            }
            else
            {
                MessageBox.Show("Ошибка", "Неправильный формат ввода ключа или количества ключей", MessageBoxButton.OK);
            }
        }
    }
}
