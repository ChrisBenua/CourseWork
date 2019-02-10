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
            mOpenMenuItem.Click += MOpenMenuItem_Click;

            RecentFiles.MachineStatesShared.LoadFilesFromDisk();
            RecentFiles.KeysCollectionShared.LoadFilesFromDisk();

            //mOpenRecentsMenuItem.ItemsSource = RecentFiles.MachineStatesShared.ShortenedNamesMenuItems;

            mOpenRecentsMenuItem.DataContext = RecentFiles.MachineStatesShared;

            mOpenRecentsMenuItem.ItemsSource = RecentFiles.MachineStatesShared.ShortenedNamesMenuItems;
            RecentFiles.MachineStatesShared.PropertyChanged += MachineStatesShared_PropertyChanged; ;
            for (int i = 0; i < mOpenRecentsMenuItem.Items.Count; ++i)
            {
                System.Windows.Controls.MenuItem menuItem = (System.Windows.Controls.MenuItem)mOpenRecentsMenuItem.Items[i];
                if (menuItem != null)
                {
                    menuItem.Click += MenuItem_Click;
                }
            }
        }

        

        private void MachineStatesShared_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            mOpenRecentsMenuItem.ItemsSource = RecentFiles.MachineStatesShared.ShortenedNamesMenuItems;

            for (int i = 0; i < mOpenRecentsMenuItem.Items.Count; ++i)
            {
                System.Windows.Controls.MenuItem menuItem = (System.Windows.Controls.MenuItem)mOpenRecentsMenuItem.Items[i];
                if (menuItem != null)
                {
                    menuItem.Click += MenuItem_Click;
                }
            }
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            //To find full path to element
            int senderIndex = mOpenRecentsMenuItem.Items.IndexOf(sender);

            Console.WriteLine((mOpenRecentsMenuItem.Items[senderIndex] as System.Windows.Controls.MenuItem).Header);

            if (senderIndex == -1)
            {
                MessageBox.Show("Странная ошибка, очень, такого не должно случаться", "Странная Ошибка");
                return;
            } 

            string openedWindowTitle = null;
            DeviceState state = null;

            (state, openedWindowTitle) = IOHelper.LoadDeviceState(RecentFiles.MachineStatesShared.RecentFileNames[senderIndex]);
            if (state != null)
            {
                Configuration.deviceState = state;

                DragAndDrop w = new DragAndDrop();
                w.CurrentFilePath = openedWindowTitle;
                w.Show();
            }
        }

        private void MOpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".xml";
            dialog.Filter = "XML-File | *.xml";
            dialog.InitialDirectory = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                string fileName = dialog.FileName;

                var tuple = IOHelper.LoadDeviceState(fileName);
                if (tuple.Item1 != null)
                {
                    Configuration.deviceState = tuple.Item1;

                    DragAndDrop w = new DragAndDrop();
                    w.CurrentFilePath = fileName;
                    w.Show();
                    this.Close();
                }
            }
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
