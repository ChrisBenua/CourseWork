using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;

namespace M138ADemo.MainClasses
{
    public class RecentFiles : INotifyPropertyChanged
    {
        public const string shortenedNamesMenuItemsPropertyName = "ShortenedNamesMenuItems";
        const string machineEndPoint = "\\machineStatesRecents.txt";
        const string keysCollectionEndPoint = "\\keysCollectionsRecents.txt";

        private List<string> _recentFileNames;

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

        private static RecentFiles machineStatesShared = null;

        private static RecentFiles keysCollectionShared = null;
        

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

        public RecentFiles()
        {
            RecentFileNames = new List<string>();
        }

        public void AddFileToRecents(string fileName)
        {
            RecentFileNames.RemoveAll((s) => s == fileName);

            RecentFileNames.Insert(0, fileName);

            RenewStoredRecentList();

            NotifyPropertyChanged("ShortenedNamesMenuItems");

        }

        public void DeleteFileFromRecents(string fileName)
        {
            RecentFileNames.RemoveAll((s) => s == fileName);

            RenewStoredRecentList();

            NotifyPropertyChanged("ShortenedNamesMenuItems");
        }

        private string getFilePath()
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

        public void LoadFilesFromDisk()
        {
            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(getFilePath()))
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

        private void RenewStoredRecentList()
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(getFilePath()))
            {
                foreach (var fileName in RecentFileNames)
                {
                    writer.WriteLine(fileName);
                }
            }

            //LoadFilesFromDisk();

        }

        public List<string> ShortenedNames
        {
            get
            {
               return RecentFileNames.ConvertAll<String>((fileName) => fileName.Split('\\').Last());
            }
        }

        public List<System.Windows.Controls.MenuItem> ShortenedNamesMenuItems
        {
            get
            {
                return ShortenedNames.ConvertAll<System.Windows.Controls.MenuItem>((fileName) =>
                {
                    var item = new System.Windows.Controls.MenuItem();
                    item.Header = fileName;
                    Console.WriteLine("\n\n" + fileName + "\n\n");
                    return item;
                });
            }
        }

        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }

    
}
