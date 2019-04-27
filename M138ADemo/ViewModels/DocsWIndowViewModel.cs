using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using M138ADemo.Models;
using System.Collections.ObjectModel;

namespace M138ADemo.ViewModels
{
    /// <summary>
    /// Documents WI ndow view model.
    /// </summary>
    public class DocsWIndowViewModel : INotifyPropertyChanged
    { 
        /// <summary>
        /// The document files.
        /// </summary>
        ObservableCollection<DocsWindowModel> _docFiles;

        /// <summary>
        /// Gets the document files.
        /// </summary>
        /// <value>The document files.</value>
        public ObservableCollection<DocsWindowModel> DocFiles => _docFiles;

        /// <summary>
        /// The selected document file.
        /// </summary>
        private DocsWindowModel _selectedDocFile;

        /// <summary>
        /// Gets or sets the selected document file.
        /// </summary>
        /// <value>The selected document file.</value>
        public DocsWindowModel SelectedDocFile
        {
            get => _selectedDocFile;

            set
            {
                _selectedDocFile = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.ViewModels.DocsWIndowViewModel"/> class.
        /// </summary>
        public DocsWIndowViewModel()
        {
            _docFiles = new ObservableCollection<DocsWindowModel>();
            _docFiles.Add(new DocsWindowModel(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\..\\..\\CryptoHistory.pdf", "История Криптографии"));
            _docFiles.Add(new DocsWindowModel(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\..\\..\\M138AExample.pdf", "О M-138-A"));
            //_docFiles.Add(new DocsWindowModel("https://cefsharp.github.io", "Git"));
        }

        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Ons the property changed.
        /// </summary>
        /// <param name="prop">Property.</param>
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
