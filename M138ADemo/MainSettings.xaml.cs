using System;
using System.Collections.Generic;
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
using System.Xml.Serialization;
using M138ADemo.MainClasses;
using M138ADemo;

namespace M138ADemo
{
    /// <summary>
    /// Interaction logic for MainSettings.xaml
    /// </summary>
    public partial class MainSettings : Window
    {

        MainSettingsViewModel viewModel = null;

        public MainSettings()
        {
            InitializeComponent();
            viewModel = new MainSettingsViewModel();
            viewModel.OnSaveHappend += (flag) =>
            {
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
                if (flag)
                {
                    this.Close();
                }
            };

            viewModel.OnOpenFileHappend += (filePath) =>
            {
                DragAndDrop w = new DragAndDrop();
                w.CurrentFilePath = filePath;
                w.Show();
                this.Close();
            };

            viewModel.OnOpenRecentFileHappend += (filePath) =>
            {
                DragAndDrop w = new DragAndDrop();
                w.CurrentFilePath = filePath;
                w.Show();
                this.Close();
            };

            //mMessagetextBox.TextChanged += MMessagetextBox_TextChanged;
            Binding binding = new Binding()
            {
                Mode = BindingMode.TwoWay,
                Source = viewModel.Model,
                Path = new PropertyPath("Message"),
                NotifyOnSourceUpdated = true,
                NotifyOnTargetUpdated = true,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            mNextButton.DataContext = viewModel;
            Binding enableBinding = new Binding()
            {
                Mode = BindingMode.OneWay,
                Path = new PropertyPath("isNextButtonEnabled"),
                NotifyOnSourceUpdated = true,
                NotifyOnTargetUpdated = true,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };

            Binding colorBinding = new Binding()
            {
                Mode = BindingMode.OneWay,
                Path = new PropertyPath("NextButtonColor"),
                NotifyOnSourceUpdated = true,
                NotifyOnTargetUpdated = true,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };

            Binding encryptBinding = new Binding()
            {
                Mode = BindingMode.OneWayToSource,
                Source = viewModel.Model,
                Path = new PropertyPath("Encrypt"),
                NotifyOnTargetUpdated = true,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };

            Binding automaticBinding = new Binding()
            {
                Mode = BindingMode.OneWayToSource,
                Source = viewModel.Model,
                Path = new PropertyPath("Automatic"),
                NotifyOnTargetUpdated = true,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };

            mEncrypt.SetBinding(RadioButton.IsCheckedProperty, encryptBinding);
            mNextButton.SetBinding(Button.BackgroundProperty, colorBinding);
            mNextButton.SetBinding(Button.IsEnabledProperty, enableBinding);
            mMessagetextBox.SetBinding(TextBox.TextProperty, binding);
            mAutomaticRadioButton.SetBinding(RadioButton.IsCheckedProperty, automaticBinding);
            mNextButton.Command = viewModel.SaveToSettingsCommand;

            mAutomaticRadioButton.IsChecked = true;

            //mNextButton.Click += mNextButtonOnClick;

            mEncrypt.IsChecked = true;
            //Configuration.Message = mMessagetextBox.Text;

            //mNextButton.IsEnabled = false;
            //mNextButton.Background = Brushes.LightGray;

            //mOpenMenuItem.Click += MOpenMenuItem_Click;
            mOpenMenuItem.Command = viewModel.OpenFileCommand;


            RecentFiles.MachineStatesShared.LoadFilesFromDisk();
            RecentFiles.KeysCollectionShared.LoadFilesFromDisk();

            //mOpenRecentsMenuItem.ItemsSource = RecentFiles.MachineStatesShared.ShortenedNamesMenuItems;

            mOpenRecentsMenuItem.DataContext = RecentFiles.MachineStatesShared;

            mOpenRecentsMenuItem.ItemsSource = RecentFiles.MachineStatesShared.ShortenedNamesMenuItems;
            RecentFiles.MachineStatesShared.PropertyChanged += MachineStatesShared_PropertyChanged;

            mOpenDocsMenuItem.Command = viewModel.OpenDocsCommand;
            SetMenuItemsCommands();
        }

        private void SetMenuItemsCommands()
        {
            for (int i = 0; i < mOpenRecentsMenuItem.Items.Count; ++i)
            {
                System.Windows.Controls.MenuItem menuItem = (System.Windows.Controls.MenuItem)mOpenRecentsMenuItem.Items[i];
                if (menuItem != null)
                {
                    menuItem.Command = viewModel.OpenRecentFileCommand;
                    menuItem.CommandParameter = i;
                }
            }
        }

        private void MachineStatesShared_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            mOpenRecentsMenuItem.ItemsSource = RecentFiles.MachineStatesShared.ShortenedNamesMenuItems;

            SetMenuItemsCommands();
        }

    }
}
