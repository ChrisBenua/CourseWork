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
using M138ADemo.Models;
using M138ADemo.ViewModels;

namespace M138ADemo
{
    /// <summary>
    /// Interaction logic for DocsWindow.xaml
    /// </summary>
    public partial class DocsWindow : Window
    {
        /// <summary>
        /// The view model.
        /// </summary>
        DocsWIndowViewModel viewModel = new DocsWIndowViewModel();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.DocsWindow"/> class.
        /// </summary>
        public DocsWindow()
        {
            InitializeComponent();
            this.DataContext = viewModel;
            //pdfWebViewer.AllowNavigation = true;
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        /// <summary>
        /// Views the model property changed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedDocFile")
            {
                //pdfWebViewer.Navigate(viewModel.SelectedDocFile.FilePath);
                pdfWebViewer.Navigate("file:///" + viewModel.SelectedDocFile.FilePath);
            }
        }
    }
}
