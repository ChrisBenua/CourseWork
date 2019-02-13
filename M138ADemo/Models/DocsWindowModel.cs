using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace M138ADemo.Models
{
    public class DocsWindowModel : INotifyPropertyChanged
    {
        string _filePath;

        public string FilePath => _filePath;

        string _shortName;

        public string ShortName => _shortName;

        public DocsWindowModel(string filepath, string shortname)
        {
            this._filePath = filepath;
            this._shortName = shortname;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

    }
}
