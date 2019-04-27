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
    /// <summary>
    /// Documents window model.
    /// </summary>
    public class DocsWindowModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The file path.
        /// </summary>
        string _filePath;

        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <value>The file path.</value>
        public string FilePath => _filePath;

        /// <summary>
        /// The short name.
        /// </summary>
        string _shortName;

        /// <summary>
        /// Gets the short name.
        /// </summary>
        /// <value>The short name.</value>
        public string ShortName => _shortName;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.Models.DocsWindowModel"/> class.
        /// </summary>
        /// <param name="filepath">Filepath.</param>
        /// <param name="shortname">Shortname.</param>
        public DocsWindowModel(string filepath, string shortname)
        {
            this._filePath = filepath;
            this._shortName = shortname;
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
