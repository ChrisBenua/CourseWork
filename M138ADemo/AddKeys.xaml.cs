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
using System.Xml;
using System.Xml.Serialization;
using M138ADemo.MainClasses;

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

            mOpenKeysMenuItem.Click += MOpenKeysMenuItem_Click;
            mSaveKeysMenuItem.Click += MSaveKeysMenuItem_Click;
        }

        private void MSaveKeysMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.InitialDirectory = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            dialog.DefaultExt = ".xml";
            dialog.Filter = "XML-File | *.xml";
            var result = dialog.ShowDialog();

            if (result == true)
            {
                string fileName = dialog.FileName;

                RecentFiles.KeysCollectionShared.AddFileToRecents(fileName);

                KeysContainer cont = new KeysContainer(Configuration.lst);
                XmlDocument doc = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(cont.GetType());

                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, cont);
                    stream.Position = 0;
                    doc.Load(stream);
                    doc.Save(fileName);
                }
            }

        }

        private void MOpenKeysMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".xml";
            dialog.Filter = "XML-File | *.xml";
            dialog.InitialDirectory = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                string fileName = dialog.FileName;
                KeysContainer state = new KeysContainer();

                XmlSerializer serializer = new XmlSerializer(state.GetType());

                StreamReader reader = new StreamReader(fileName);
                state = (KeysContainer)serializer.Deserialize(reader);

                Configuration.lst.Clear();

                foreach (var el in state.keys)
                {
                    Configuration.lst.Add(Pair<int, string>.MakePair(el.Id, el.Key));
                }

                reader.Close();
            }
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
