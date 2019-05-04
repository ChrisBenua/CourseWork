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
using M138ADemo.ViewModels;

namespace M138ADemo
{
    /// <summary>
    /// Interaction logic for DecryptAddKeys.xaml
    /// </summary>
    public partial class DecryptAddKeys : Window
    {
        /// <summary>
        /// The view model.
        /// </summary>
        private DecryptAddKeysViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.DecryptAddKeys"/> class.
        /// </summary>
        public DecryptAddKeys()
        {
            InitializeComponent();
            viewModel = new DecryptAddKeysViewModel();

            mOpenFromFileMenuItem.Command = viewModel.OpenKeysFromFile;
            mSaveFileMenuItem.Command = viewModel.SaveKeysCommand;
            mBackButton.Click += MBackButton_Click;

            //mColumnNumberTextBox.TextChanged += MColumnNumberTextBox_TextChanged;
            mAddKeysForPair.Click += MAddKeysForPair_Click;
            mAddUserKeysButton.Click += MAddUserKeysButton_Click;
            mNextButton.Click += MNextButton_Click;
            mShowKeysButton.Click += MShowKeysButton_Click;
            mAddTodayKeysButton.Click += MAddTodayKeysButton_Click;
            mAddRandomKeysButton.Click += MAddRandomKeysButton_Click;

            mColumnNumberTextBox.SetBinding(TextBox.TextProperty, new Binding()
            {
                Source = viewModel,
                Path = new PropertyPath("ColumnNumberText"),
                NotifyOnSourceUpdated = true,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

            Binding buttonDisableBinding = new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("IsNextButtonEnabled"),
                NotifyOnSourceUpdated = true,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            mNextButton.SetBinding(Button.IsEnabledProperty, buttonDisableBinding);

            Binding buttonColorBinding = new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("NextButtonColor"),
                NotifyOnSourceUpdated = true,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            mNextButton.SetBinding(Button.BackgroundProperty, buttonColorBinding);

            Binding warningBinding = new Binding
            {
                Source = viewModel.Keys,
                Path = new PropertyPath("Count"),
                Converter = new WarningIntToStringConverter(),
                NotifyOnSourceUpdated = true,
                NotifyOnTargetUpdated = true,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            mWarningTextBlock.SetBinding(TextBlock.TextProperty, warningBinding);

            Binding numberOfKeysBinding = new Binding
            {
                Source = viewModel.Keys,
                Path = new PropertyPath("Count"),
                Converter = new IntToStringConverter("Кол-во ключей: "),
                BindsDirectlyToSource = true,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            mNumberOfKeysTextBlock.SetBinding(TextBlock.TextProperty, numberOfKeysBinding);
            mOpenRecentMenutItem.SetBinding(MenuItem.ItemsSourceProperty, new Binding
            {
                Source = viewModel.MenuItems,
                NotifyOnSourceUpdated = true,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
        }

        private void MAddRandomKeysButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new RandomKeysWindow();
            window.Closed += (o, ev) => this.viewModel.PerformValidation();
            window.Show();
        }

        private void MAddTodayKeysButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new TodayKeysWindow();
            window.Closed += (o, ev) => this.viewModel.PerformValidation();

            window.Show();
        }

        private void MShowKeysButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new ShowKeys();
            window.Closed += (o, ev) => this.viewModel.PerformValidation();
            window.Show();
        }

        private void MBackButton_Click(object sender, RoutedEventArgs e)
        {
            //Configuration.lst.Clear();
            MainSettings w = new MainSettings();
            w.Show();
            this.Close();
        }

        private void MNextButton_Click(object sender, RoutedEventArgs e)
        {
            Configuration.DecryptIndex = viewModel.columnNumber;
            DragAndDrop w = new DragAndDrop();
            w.Show();
            this.Close();
        }

        private void MAddUserKeysButton_Click(object sender, RoutedEventArgs e)
        {
            ShowKeys w = new ShowKeys();
            w.Closed += (s, args) => viewModel.PerformValidation();
            w.Show();

        }

        private void MAddKeysForPair_Click(object sender, RoutedEventArgs e)
        {
            KeysForPair w = new KeysForPair();
            w.Closed += (s, args) => viewModel.PerformValidation();
            w.Show();
        }
    }
}
