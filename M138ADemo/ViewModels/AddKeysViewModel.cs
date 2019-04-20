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


    public class AddKeysViewModel : INotifyPropertyChanged
    {


        public enum WindowsToBeOpened
        {
            NextButton, TodayKeysButton, RandomKeysButton, PairKeysButton, ShowKeysButton, AddUsersKeysButton, MainSettings
        }

        private IDialogService dialogService;

        ObservableCollection<Pair<int, string>> _keys = new ObservableCollection<Pair<int, string>>();

        public ObservableCollection<Pair<int, string>> Keys
        {
            get => _keys;

            private set
            {
                _keys = value;
                OnPropertyChanged();
            }
        }

        private string _keysFileName;

        public string KeysFileName
        {
            get => _keysFileName;

            private set
            {
                _keysFileName = value;
                OnPropertyChanged();
            }
        }

        private List<MenuItem> _menuItems;

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

        private RelayCommand _openRecentFileCommand;

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
                        this.Keys.Add(new Pair<int, string>(el.Id, el.Key));
                    }
                }));
            }
        }

        private RelayCommand _openFileCommand;

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
                            this.Keys.Add(Pair<int, string>.MakePair(el.Id, el.Key));
                        }
                    }
                }));
            }
        }

        private RelayCommand _saveFileCommand;

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

        private RelayCommand _openAnotherWindowCommand;

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

        public AddKeysViewModel()
        {
            if (!(Configuration.lst is null))
            {
                this.Keys = Configuration.lst;
            }
        }

        public void SaveToConfiguration()
        {
            Configuration.lst = this.Keys;

        }

        private void KeysCollectionShared_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == RecentFiles.shortenedNamesMenuItemsPropertyName)
            {
                MenuItems = RecentFiles.KeysCollectionShared.ShortenedNamesMenuItems;
            }
        }

        public AddKeysViewModel(IDialogService dialogService): this()
        {
            this.dialogService = dialogService;
            RecentFiles.KeysCollectionShared.PropertyChanged += KeysCollectionShared_PropertyChanged;
            MenuItems = RecentFiles.KeysCollectionShared.ShortenedNamesMenuItems;
        }

        public event Action NotifyToCloseEvent;

        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
