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
    /// Interaction logic for AddKeys.xaml
    /// </summary>
    public partial class AddKeys : Window
    {
        public AddKeys()
        {
            InitializeComponent();
           AddUserKeyButton.Click += AddUserKeyButtonOnClick;
            ShowKeysButton.Click += ShowKeysButtonOnClick;
            PairKeysButton.Click += PairKeysButtonOnClick;
            RandomKeyButton.Click += RandomKeyButton_Click;
            TodayKeyButton.Click += TodayKeyButton_Click;
            mNextButton.Click += MNextButton_Click;
            //mNumberOfKeysSelected.DataContext = Configuration.lst;
            Binding binding = new Binding()
            {
                Source = Configuration.lst,
                Path = new PropertyPath("Count"),
                //Mode = BindingMode.OneWay,
                Converter = new IntToStringConverter("Кол-во ключей:"),
                BindsDirectlyToSource = true,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };

            mNumberOfKeysSelected.SetBinding(TextBlock.TextProperty,binding);

            Binding warningBinding = new Binding
            {
                Source = Configuration.lst,
                Path = new PropertyPath("Count"),
                Converter = new WarningIntToStringConverter(),
                BindsDirectlyToSource = true,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            mWarninTextBlock.SetBinding(TextBlock.TextProperty, warningBinding);

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
        }

        private void MNextButton_Click(object sender, RoutedEventArgs e)
        {
            DragAndDrop w = new DragAndDrop();
            w.Show();
            this.Close();
        }

        private void TodayKeyButton_Click(object sender, RoutedEventArgs e)
        {
            TodayKeysWindow w = new TodayKeysWindow();
            w.Show();
        }

        private void RandomKeyButton_Click(object sender, RoutedEventArgs e)
        {
            RandomKeysWindow w = new RandomKeysWindow();
            w.Show();
        }

        private void PairKeysButtonOnClick(object sender, RoutedEventArgs e)
        {
            KeysForPair w = new KeysForPair();
            w.Show();
        }

        private void ShowKeysButtonOnClick(object sender, RoutedEventArgs e)
        {
            ShowKeys w = new ShowKeys();
            w.Show();
        }

        private void AddUserKeyButtonOnClick(object sender, RoutedEventArgs e)
        {
            AddUsersKey w = new AddUsersKey();
            w.Show();
        }
    }
}
