using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;

namespace M138ADemo.MainClasses
{
    /// <summary>
    /// Recent files.
    /// </summary>
    public class RecentFiles : INotifyPropertyChanged
    {
        /// <summary>
        /// The name of the shortened names menu items property.
        /// </summary>
        public const string shortenedNamesMenuItemsPropertyName = "ShortenedNamesMenuItems";

        /// <summary>
        /// The machine end point.
        /// </summary>
        const string machineEndPoint = "\\machineStatesRecents.txt";

        /// <summary>
        /// The keys collection end point.
        /// </summary>
        const string keysCollectionEndPoint = "\\keysCollectionsRecents.txt";
        /// <summary>
        /// The max capacity of recents files.
        /// </summary>
        const int maxCapacity = 6;

        /// <summary>
        /// The recent file names.
        /// </summary>
        private List<string> _recentFileNames;

        /// <summary>
        /// Gets the recent file names.
        /// </summary>
        /// <value>The recent file names.</value>
        public List<string> RecentFileNames
        {
            get => _recentFileNames;

            private set
            {
                _recentFileNames = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("ShortenedNames");
                NotifyPropertyChanged(shortenedNamesMenuItemsPropertyName);
            }
        }

        /// <summary>
        /// The machine states shared.
        /// </summary>
        private static RecentFiles machineStatesShared = null;


        /// <summary>
        /// The keys collection shared.
        /// </summary>
        private static RecentFiles keysCollectionShared = null;
        
        /// <summary>
        /// Gets the machine states shared.
        /// </summary>
        /// <value>The machine states shared.</value>
        public static RecentFiles MachineStatesShared
        {
            get
            {
                if (machineStatesShared == null)
                {
                    machineStatesShared = new RecentFiles();
                }
                return machineStatesShared;
            }
        }

        /// <summary>
        /// Gets the keys collection shared.
        /// </summary>
        /// <value>The keys collection shared.</value>
        public static RecentFiles KeysCollectionShared
        {
            get
            {
                if (keysCollectionShared == null)
                {
                    keysCollectionShared = new RecentFiles();
                }
                return keysCollectionShared;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.MainClasses.RecentFiles"/> class.
        /// </summary>
        public RecentFiles()
        {
            RecentFileNames = new List<string>();
        }

        /// <summary>
        /// Adds the file to recents.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public void AddFileToRecents(string fileName)
        {
            RecentFileNames.RemoveAll((s) => s == fileName);
            RecentFileNames.Insert(0, fileName);
            RenewStoredRecentList();
            NotifyPropertyChanged("ShortenedNamesMenuItems");
        }

        /// <summary>
        /// Deletes the file from recents.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public void DeleteFileFromRecents(string fileName)
        {
            RecentFileNames.RemoveAll((s) => s == fileName);
            RenewStoredRecentList();
            NotifyPropertyChanged("ShortenedNamesMenuItems");
        }

        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <returns>The file path.</returns>
        private string GetFilePath()
        {
            string filePath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            if (this == machineStatesShared)
            {
                filePath += machineEndPoint;
            }
            else
            {
                filePath += keysCollectionEndPoint;
            }
            return filePath;
        }

        /// <summary>
        /// Loads the files from disk.
        /// </summary>
        public void LoadFilesFromDisk()
        {
            var fileInfo = new System.IO.FileInfo(GetFilePath());
            if (!fileInfo.Exists)
            {
                fileInfo.Create();
            }
            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(GetFilePath()))
                {
                    while (!reader.EndOfStream)
                    {
                        string newFileName = reader.ReadLine();
                        RecentFileNames.Add(newFileName);
                    }
                }
            } catch (System.IO.IOException ex)
            {
                Console.WriteLine("\n\n TXT FILES TO STORE RECENTS \n\n");
                MessageBox.Show($"Произошла ошибка при чтении из файла, убедитесь, что вы не удалили и не перенесли файл {keysCollectionEndPoint} или {machineEndPoint}", "Ошибка", MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// Renews the stored recent list.
        /// </summary>
        private void RenewStoredRecentList()
        {
            this.RecentFileNames = this.RecentFileNames.Distinct().ToList();
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(GetFilePath()))
            {
                foreach (var fileName in RecentFileNames)
                {
                    writer.WriteLine(fileName);
                }
            }
        }

        /// <summary>
        /// Gets the shortened names.
        /// </summary>
        /// <value>The shortened names.</value>
        public List<string> ShortenedNames
        {
            get
            {
               return RecentFileNames.Distinct().ToList().ConvertAll<String>((fileName) => fileName.Split('\\').Last());
            }
        }

        /// <summary>
        /// Gets the shortened names prefix.
        /// </summary>
        /// <value>The shortened names prefix.</value>
        public List<string> ShortenedNamesPrefix
        {
            get
            {
                return RecentFileNames.Distinct().Take(maxCapacity).ToList().ConvertAll<String>((filename) => filename.Split('\\').Last());
            }
        }

        /// <summary>
        /// Gets the shortened names menu items.
        /// </summary>
        /// <value>The shortened names menu items.</value>
        public List<System.Windows.Controls.MenuItem> ShortenedNamesMenuItems
        {
            get
            {
                return ShortenedNamesPrefix.ConvertAll<System.Windows.Controls.MenuItem>((fileName) =>
                {
                    var item = new System.Windows.Controls.MenuItem();
                    item.Header = fileName;
                    Console.WriteLine("\n\n" + fileName + "\n\n");
                    return item;
                });
            }
        }

        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

    }

    
}
