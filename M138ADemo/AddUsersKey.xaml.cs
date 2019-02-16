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
using M138ADemo.MainClasses;
using M138ADemo.ViewModels;

namespace M138ADemo
{
    /// <summary>
    /// Interaction logic for AddUsersKey.xaml
    /// </summary>
    public partial class AddUsersKey : Window
    {
        

        private AddUsersKeysViewModel viewModel;
        private ShowAndAddKeysInteractor interactor;

        public AddUsersKey()
        {
            InitializeComponent();
            viewModel = new AddUsersKeysViewModel();
            interactor = new ShowAndAddKeysInteractor(dataGrid, viewModel);

            viewModel.NotifyOnClose += () => this.Close();
            mAddKeyButton.Command = viewModel.AddkeyCommand;

            mEndButton.Command = viewModel.EndCommand;

        }
    }
}
