using M138ADemo.MainClasses;
using M138ADemo.ViewModels;
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
    /// Interaction logic for ShowKeys.xaml
    /// </summary>
    public partial class ShowKeys : Window
    {
        private ShowKeysViewModel viewModel;
        private ShowAndAddKeysInteractor interactor;
        public ShowKeys() 
        {
            InitializeComponent();

            viewModel = new ShowKeysViewModel();
            interactor = new ShowAndAddKeysInteractor(dataGrid, viewModel);

            viewModel.NotifyOnClose += () => this.Close();
            mAddKeyButton.Command = viewModel.AddkeyCommand;

            mEndButton.Command = viewModel.EndCommand;
        }
    }
}

