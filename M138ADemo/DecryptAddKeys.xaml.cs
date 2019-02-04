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
    /// Interaction logic for DecryptAddKeys.xaml
    /// </summary>
    public partial class DecryptAddKeys : Window
    {
        public DecryptAddKeys()
        {
            InitializeComponent();
            mColumnNumberTextBox.TextChanged += MColumnNumberTextBox_TextChanged;
            mAddKeysForPair.Click += MAddKeysForPair_Click;
            mAddUserKeysButton.Click += MAddUserKeysButton_Click;
            mNextButton.Click += MNextButton_Click;

            Binding buttonDisableBinding = new Binding
            {
                Source = Configuration.lst,
                Path = new PropertyPath("Count"),
                Converter = new WarningIntToBoolConverter(),
                BindsDirectlyToSource = true,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            mNextButton.SetBinding(Button.IsEnabledProperty, buttonDisableBinding);

            Binding buttonColorBinding = new Binding
            {
                Source = Configuration.lst,
                Path = new PropertyPath("Count"),
                Converter = new IsEnabledToColorConverter(Brushes.Aqua, Brushes.LightGray),
                BindsDirectlyToSource = true,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            mNextButton.SetBinding(Button.BackgroundProperty, buttonColorBinding);

            Binding warningBinding = new Binding
            {
                Source = Configuration.lst,
                Path = new PropertyPath("Count"),
                Converter = new WarningIntToStringConverter(),
                BindsDirectlyToSource = true,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            mWarningTextBlock.SetBinding(TextBlock.TextProperty, warningBinding);

            Binding numberOfKeysBinding = new Binding
            {
                Source = Configuration.lst,
                Path = new PropertyPath("Count"),
                Converter = new IntToStringConverter("Кол-во ключей:"),
                BindsDirectlyToSource = true,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            mNumberOfKeysTextBlock.SetBinding(TextBlock.TextProperty, numberOfKeysBinding);

        }

        private void MNextButton_Click(object sender, RoutedEventArgs e)
        {
            Configuration.DecryptIndex = int.Parse(mColumnNumberTextBox.Text);
            DragAndDrop w = new DragAndDrop();
            w.Show();
            this.Close();
        }

        private void MAddUserKeysButton_Click(object sender, RoutedEventArgs e)
        {
            AddUsersKey w = new AddUsersKey();
            w.Closed += (s, args) => MColumnNumberTextBox_TextChanged(null, null);
            w.Show();

        }

        private void MAddKeysForPair_Click(object sender, RoutedEventArgs e)
        {
            KeysForPair w = new KeysForPair();
            w.Closed += (s, args) => MColumnNumberTextBox_TextChanged(null, null);
            w.Show();
        }

        private void MColumnNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            WarningIntToStringConverter conv = new WarningIntToStringConverter();

            mColumnNumberTextBox.Text = new string((from ch in mColumnNumberTextBox.Text where (ch >= '0' && ch <= '9') select ch).ToArray());
            int num = 0;
            if (!int.TryParse(mColumnNumberTextBox.Text, out num))
            {
                if (mColumnNumberTextBox.Text.Length > 0)
                {
                    MessageBox.Show("Неправильный формат, перепроверьте", "Ошибка", MessageBoxButton.OK);
                }
                mNextButton.IsEnabled = false;
                mNextButton.Background = Brushes.LightGray;
                //mWarningTextBlock.Text = (string)conv.Convert(0, "".GetType(), new object(), System.Globalization.CultureInfo.CurrentCulture);
            }
            else if (num > 26)
            {
                MessageBox.Show("Слишком большое число, оно должно быть меньше 27", "Ошибка", MessageBoxButton.OK);
                mNextButton.IsEnabled = false;
                mNextButton.Background = Brushes.LightGray;
                //mWarningTextBlock.Text = (string)conv.Convert(0, "".GetType(), new object(), System.Globalization.CultureInfo.CurrentCulture);

            }
            else
            {
                if (Configuration.lst.Count >= Configuration.Message.Length)
                {
                    mNextButton.IsEnabled = true;
                    mNextButton.Background = Brushes.Aqua;
                }
                mWarningTextBlock.Text = (string)conv.Convert(Configuration.lst.Count, "".GetType(), new object(), System.Globalization.CultureInfo.CurrentCulture);

            }
        }
    }
}
