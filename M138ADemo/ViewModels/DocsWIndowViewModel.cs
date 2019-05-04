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
            var path1 = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\..\\..\\CryptoHistory.pdf";
            var fileInfo1 = new System.IO.FileInfo(path1);
            if (fileInfo1.Exists)
            {
                _docFiles.Add(new DocsWindowModel(path1, "История Криптографии"));
            }
            else
            {
                _docFiles.Add(new DocsWindowModel(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\CryptoHistory.pdf", "История Криптографии"));
            }

            var path2 = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\..\\..\\M138AExample.pdf";
            var fileInfo2 = new System.IO.FileInfo(path2);

            if (fileInfo2.Exists)
            {
                _docFiles.Add(new DocsWindowModel(path2, "О M-138-A"));

            }
            else
            {
                _docFiles.Add(new DocsWindowModel(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\M138AExample.pdf", "О M-138-A"));
            }
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
