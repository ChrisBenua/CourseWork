using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using M138ADemo.MainClasses;
using M138ADemo.Services;

namespace M138ADemo.ViewModels
{

    /// <summary>
    /// Add keys view model.
    /// </summary>
    public class AddKeysViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Windows to be opened.
        /// </summary>
        public enum WindowsToBeOpened
        {
            NextButton, TodayKeysButton, RandomKeysButton, PairKeysButton, ShowKeysButton, AddUsersKeysButton, MainSettings
        }

        /// <summary>
        /// The dialog service.
        /// </summary>
        private IDialogService dialogService;

        /// <summary>
        /// The keys.
        /// </summary>
        ObservableCollection<(int, string)> _keys = new ObservableCollection<(int, string)>();

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <value>The keys.</value>
        public ObservableCollection<(int, string)> Keys
        {
            get => _keys;

            private set
            {
                _keys = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The name of the keys file.
        /// </summary>
        private string _keysFileName;

        /// <summary>
        /// Gets the name of the keys file.
        /// </summary>
        /// <value>The name of the keys file.</value>
        public string KeysFileName
        {
            get => _keysFileName;

            private set
            {
                _keysFileName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The menu items.
        /// </summary>
        private List<MenuItem> _menuItems;

        /// <summary>
        /// Gets the menu items.
        /// </summary>
        /// <value>The menu items.</value>
        public List<MenuItem> MenuItems
        {
            get
            {
                return _menuItems;
            }

            private set
            {
                _menuItems = value;
                for (int i = 0; i < _menuItems.Count; ++i)
                {
                    _menuItems[i].Command = this.OpenRecentFileCommand;
                    _menuItems[i].CommandParameter = i;
                }
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The open recent file command.
        /// </summary>
        private RelayCommand _openRecentFileCommand;

        /// <summary>
        /// Gets the open recent file command.
        /// </summary>
        /// <value>The open recent file command.</value>
        public RelayCommand OpenRecentFileCommand
        {
            get
            {
                return _openRecentFileCommand ?? (_openRecentFileCommand = new RelayCommand(obj =>
                {
                    int callerIndex = (int)obj;

                    string filePath = RecentFiles.KeysCollectionShared.RecentFileNames[callerIndex];
                    this.Keys.Clear();
                    foreach (var el in IOHelper.LoadKeysContainer(filePath).keys)
                    {
                        this.Keys.Add((el.Id, el.Key));
                    }
                }));
            }
        }

        /// <summary>
        /// The open file command.
        /// </summary>
        private RelayCommand _openFileCommand;

        /// <summary>
        /// Gets the open file command.
        /// </summary>
        /// <value>The open file command.</value>
        public RelayCommand OpenFileCommand
        {
            get
            {
                return _openFileCommand ?? (_openFileCommand = new RelayCommand(obj =>
                {
                    if (dialogService.OpenFileDialog())
                    {
                        this.Keys.Clear();
                        foreach (var el in IOHelper.LoadKeysContainer(dialogService.FilePath).keys)
                        {
                            this.Keys.Add((el.Id, el.Key));
                        }
                    }
                }));
            }
        }

        /// <summary>
        /// The save file command.
        /// </summary>
        private RelayCommand _saveFileCommand;

        /// <summary>
        /// Gets the save file command.
        /// </summary>
        /// <value>The save file command.</value>
        public RelayCommand SaveFileCommand
        {
            get
            {
                return _saveFileCommand ?? (_saveFileCommand = new RelayCommand(obj =>
                {
                    if (dialogService.SaveFileDialog())
                    {
                        IOHelper.SaveKeysContainer(dialogService.FilePath, Keys);
                        this.KeysFileName = dialogService.FilePath;
                    }
                }));
            }
        }

        /// <summary>
        /// The open another window command.
        /// </summary>
        private RelayCommand _openAnotherWindowCommand;

        /// <summary>
        /// Gets the open another window command.
        /// </summary>
        /// <value>The open another window command.</value>
        public RelayCommand OpenAnotherWindowCommand
        {
            get
            {
                return _openAnotherWindowCommand ?? (_openAnotherWindowCommand = new RelayCommand(obj =>
                {
                    WindowsToBeOpened destination = (WindowsToBeOpened)obj;
                    this.SaveToConfiguration();
                    switch(destination)
                    {
                        case WindowsToBeOpened.NextButton:
                            DragAndDrop w = new DragAndDrop();
                            w.Show();
                            NotifyToCloseEvent?.Invoke();
                            break;
                        case WindowsToBeOpened.PairKeysButton:
                            KeysForPair w1 = new KeysForPair();
                            w1.Show();
                            break;
                        case WindowsToBeOpened.RandomKeysButton:
                            RandomKeysWindow w2 = new RandomKeysWindow();
                            w2.Show();
                            break;
                        case WindowsToBeOpened.ShowKeysButton:
                            ShowKeys w3 = new ShowKeys();
                            w3.Show();
                            break;
                        case WindowsToBeOpened.TodayKeysButton:
                            TodayKeysWindow w4 = new TodayKeysWindow();
                            w4.Show();
                            break;
                        case WindowsToBeOpened.AddUsersKeysButton:
                            AddUsersKey w5 = new AddUsersKey();
                            w5.Show();
                            break;
                        case WindowsToBeOpened.MainSettings:
                            MainSettings w6 = new MainSettings();
                            w6.Show();
                            NotifyToCloseEvent?.Invoke();
                            break;
                    }
                }));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.ViewModels.AddKeysViewModel"/> class.
        /// </summary>
        public AddKeysViewModel()
        {
            if (!(Configuration.KeyList is null))
            {
                this.Keys = Configuration.KeyList;
            }
        }

        /// <summary>
        /// Saves to configuration.
        /// </summary>
        public void SaveToConfiguration()
        {
            Configuration.KeyList = this.Keys;

        }

        /// <summary>
        /// Keyses the collection shared property changed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void KeysCollectionShared_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == RecentFiles.shortenedNamesMenuItemsPropertyName)
            {
                MenuItems = RecentFiles.KeysCollectionShared.ShortenedNamesMenuItems;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.ViewModels.AddKeysViewModel"/> class.
        /// </summary>
        /// <param name="dialogService">Dialog service.</param>
        public AddKeysViewModel(IDialogService dialogService): this()
        {
            this.dialogService = dialogService;
            RecentFiles.KeysCollectionShared.PropertyChanged += KeysCollectionShared_PropertyChanged;
            MenuItems = RecentFiles.KeysCollectionShared.ShortenedNamesMenuItems;
        }

        /// <summary>
        /// Occurs when notify to close event.
        /// </summary>
        public event Action NotifyToCloseEvent;

        /// <summary>
        /// Ons the property changed.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
