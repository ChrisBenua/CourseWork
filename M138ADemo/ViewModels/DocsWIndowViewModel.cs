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
    public class DocsWIndowViewModel : INotifyPropertyChanged
    {

        ObservableCollection<DocsWindowModel> _docFiles;

        public ObservableCollection<DocsWindowModel> DocFiles => _docFiles;

        private DocsWindowModel _selectedDocFile;

        public DocsWindowModel SelectedDocFile
        {
            get => _selectedDocFile;

            set
            {
                _selectedDocFile = value;
                OnPropertyChanged();
            }
        }

        public DocsWIndowViewModel()
        {
            _docFiles = new ObservableCollection<DocsWindowModel>();
            _docFiles.Add(new DocsWindowModel(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\HomeworkDiscrete3module.pdf", "Variants"));
            //_docFiles.Add(new DocsWindowModel("https://cefsharp.github.io", "Git"));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
