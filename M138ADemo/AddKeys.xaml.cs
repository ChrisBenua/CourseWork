using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using System.Xml;
using System.Xml.Serialization;
using M138ADemo.MainClasses;
using M138ADemo.ViewModels;
using M138ADemo.Services;

namespace M138ADemo
{
    /// <summary>
    /// Interaction logic for AddKeys.xaml
    /// </summary>
    public partial class AddKeys : Window
    {
        /// <summary>
        /// The view model.
        /// </summary>
        private AddKeysViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.AddKeys"/> class.
        /// </summary>
        public AddKeys()
        {
            InitializeComponent();
            viewModel = new AddKeysViewModel(new DefaultDialogService());

            mBackButton.Command = viewModel.OpenAnotherWindowCommand;
            mBackButton.CommandParameter = AddKeysViewModel.WindowsToBeOpened.MainSettings;

            AddUserKeyButton.Command = viewModel.OpenAnotherWindowCommand;
            AddUserKeyButton.CommandParameter = AddKeysViewModel.WindowsToBeOpened.AddUsersKeysButton;

            ShowKeysButton.Command = viewModel.OpenAnotherWindowCommand;
            ShowKeysButton.CommandParameter = AddKeysViewModel.WindowsToBeOpened.ShowKeysButton;

            PairKeysButton.Command = viewModel.OpenAnotherWindowCommand;
            PairKeysButton.CommandParameter = AddKeysViewModel.WindowsToBeOpened.PairKeysButton;

            RandomKeyButton.Command = viewModel.OpenAnotherWindowCommand;
            RandomKeyButton.CommandParameter = AddKeysViewModel.WindowsToBeOpened.RandomKeysButton;

            TodayKeyButton.Command = viewModel.OpenAnotherWindowCommand;
            TodayKeyButton.CommandParameter = AddKeysViewModel.WindowsToBeOpened.TodayKeysButton;

            mNextButton.Command = viewModel.OpenAnotherWindowCommand;
            mNextButton.CommandParameter = AddKeysViewModel.WindowsToBeOpened.NextButton;

            Binding binding = new Binding()
            {
                Source = viewModel.Keys,
                Path = new PropertyPath("Count"),
                //Mode = BindingMode.OneWay,
                Converter = new IntToStringConverter("Кол-во ключей: "),
                BindsDirectlyToSource = true,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            mNumberOfKeysSelected.SetBinding(TextBlock.TextProperty, binding);

            Binding warningBinding = new Binding
            {
                Source = viewModel.Keys,
                Path = new PropertyPath("Count"),
                Converter = new WarningIntToStringConverter(),
                BindsDirectlyToSource = true,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            mWarninTextBlock.SetBinding(TextBlock.TextProperty, warningBinding);

            Binding buttonDisableBinding = new Binding
            {
                Source = viewModel.Keys,
                Path = new PropertyPath("Count"),
                Converter = new WarningIntToBoolConverter(),
                BindsDirectlyToSource = true,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            mNextButton.SetBinding(Button.IsEnabledProperty, buttonDisableBinding);

            Binding buttonColorBinding = new Binding
            {
                Source = viewModel.Keys,
                Path = new PropertyPath("Count"),
                Converter = new IsEnabledToColorConverter(Brushes.Aqua, Brushes.LightGray),
                BindsDirectlyToSource = true,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            mNextButton.SetBinding(Button.BackgroundProperty, buttonColorBinding);

            mOpenKeysMenuItem.Command = viewModel.OpenFileCommand;
            mSaveKeysMenuItem.Command = viewModel.SaveFileCommand;

            Binding openRecentFilesMenuItemsBinding = new Binding
            {
                Source = viewModel.MenuItems,
                BindsDirectlyToSource = true,
                NotifyOnSourceUpdated = true,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };

            mOpenRecentKeysMenuItem.SetBinding(MenuItem.ItemsSourceProperty, openRecentFilesMenuItemsBinding);

            viewModel.NotifyToCloseEvent += ViewModel_NotifyToCloseEvent;
        }

        /// <summary>
        /// Views the model notify to close event.
        /// </summary>
        private void ViewModel_NotifyToCloseEvent()
        {
            this.Close();
        }  
    }
}
