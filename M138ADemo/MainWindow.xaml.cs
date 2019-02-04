using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace M138ADemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///

    

    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            
            InitializeComponent();
        }

        private void MNumberOfKeys_OnPreviewTextInput(object sender, TextCompositionEventArgs eArgs)
        {
            Regex reg = new Regex("[^0-9]+");
            eArgs.Handled = reg.IsMatch(eArgs.Text) && eArgs.Text.Length < 4;
        }

        private void AddUsersKeys(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void PrivateKeyGeneration(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TodayKeysGeneration(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
