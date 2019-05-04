using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using M138ADemo.Services;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using M138ADemo.MainClasses;

namespace M138ADemo.ViewModels
{
    /// <summary>
    /// Decrypt add keys view model.
    /// </summary>
    public class DecryptAddKeysViewModel: INotifyPropertyChanged
    {
        /// <summary>
        /// The dialog service.
        /// </summary>
        private IDialogService dialogService;


        private ObservableCollection<System.Windows.Controls.MenuItem> _menuItems;

        public ObservableCollection<System.Windows.Controls.MenuItem> MenuItems
        {
            get => _menuItems;

            private set
            {
                _menuItems = value;
                SetCommands();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Sets Command and its parametr when collection changes
        /// </summary>
        private void SetCommands()
        {
            for (int i = 0; i < _menuItems.Count; ++i)
            {
                _menuItems[i].Command = this.OpenRecentFileCommand;
                _menuItems[i].CommandParameter = i;
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
                    Console.WriteLine("Opening File " + filePath);
                    this.Keys.Clear();
                    foreach (var el in IOHelper.LoadKeysContainer(filePath).keys)
                    {
                        this.Keys.Add((el.Id, el.Key));
                    }
                }));
            }
        }

        /// <summary>
        /// The keys.
        /// </summary>
        private ObservableCollection<(int, string)> _keys;

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <value>The keys.</value>
        public ObservableCollection<(int, string)> Keys
        {
            get
            {
                return _keys;
            }
            
            private set
            {
                _keys = value;
                //ColumnNumberText = _columnNumberText ?? "";
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The column number.
        /// </summary>
        public int columnNumber = -1;

        /// <summary>
        /// The column number text.
        /// </summary>
        private string _columnNumberText;

        /// <summary>
        /// Performs the validation.
        /// </summary>
        public void PerformValidation()
        {
            if (!int.TryParse(_columnNumberText, out columnNumber))
            {
                if (_columnNumberText.Length > 0)
                {
                    dialogService.ShowMessage("Неправильный формат, перепроверьте", "Ошибка");
                }

                if (Configuration.Manual && Keys.Count >= Configuration.Message.Length)
                {
                    IsNextButtonEnabled = true;
                    NextButtonColor = Brushes.Aqua;
                }
                else
                {
                    IsNextButtonEnabled = false;
                    NextButtonColor = Brushes.LightGray;
                }
            }
            else if (columnNumber > 26)
            {
                dialogService.ShowMessage("Слишком большое число, оно должно быть меньше 27", "Ошибка");
                IsNextButtonEnabled = false;
                NextButtonColor = Brushes.LightGray;

            }
            else
            {
                if (Keys.Count >= Configuration.Message.Length)
                {
                    IsNextButtonEnabled = true;
                    NextButtonColor = Brushes.Aqua;
                }
            }
        }

        /// <summary>
        /// Gets or sets the column number text.
        /// </summary>
        /// <value>The column number text.</value>
        public string ColumnNumberText
        {
            get
            {
                return _columnNumberText;
            }
            set
            {
                _columnNumberText = value;

                _columnNumberText = new string((from ch in _columnNumberText where (ch >= '0' && ch <= '9') select ch).ToArray());

                PerformValidation();

                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The color of the next button.
        /// </summary>
        private Brush _nextButtonColor;

        /// <summary>
        /// Gets or sets the color of the next button.
        /// </summary>
        /// <value>The color of the next button.</value>
        public Brush NextButtonColor
        {
            get => _nextButtonColor;

            set
            {
                _nextButtonColor = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The is next button enabled.
        /// </summary>
        private bool _isNextButtonEnabled;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:M138ADemo.ViewModels.DecryptAddKeysViewModel"/> is
        /// next button enabled.
        /// </summary>
        /// <value><c>true</c> if is next button enabled; otherwise, <c>false</c>.</value>
        public bool IsNextButtonEnabled
        {
            get
            {
                return _isNextButtonEnabled;
            }

            set
            {
                _isNextButtonEnabled = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The open keys from file.
        /// </summary>
        private RelayCommand _openKeysFromFile;

        /// <summary>
        /// Gets the open keys from file.
        /// </summary>
        /// <value>The open keys from file.</value>
        public RelayCommand OpenKeysFromFile
        {
            get
            {
                return _openKeysFromFile ?? (_openKeysFromFile = new RelayCommand(obj =>
                {
                    if (dialogService.OpenFileDialog())
                    {
                        this.Keys.Clear();
                        foreach (var el in IOHelper.LoadKeysContainer(dialogService.FilePath).keys)
                        {
                            this.Keys.Add((el.Id, el.Key));
                        }
                        PerformValidation();
                    }
                }));
            }
        }

        /// <summary>
        /// The save file command.
        /// </summary>
        private RelayCommand _saveFileCommand;

        /// <summary>
        /// Gets the save keys command.
        /// </summary>
        /// <value>The save keys command.</value>
        public RelayCommand SaveKeysCommand
        {
            get
            {
                return _saveFileCommand ?? (_saveFileCommand = new RelayCommand(obj =>
                {
                    if (dialogService.SaveFileDialog())
                    {
                        IOHelper.SaveKeysContainer(dialogService.FilePath, Keys);
                    }
                }));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.ViewModels.DecryptAddKeysViewModel"/> class.
        /// </summary>
        public DecryptAddKeysViewModel()
        {
            this.Keys = Configuration.KeyList;
            
            this.NextButtonColor = Brushes.LightGray;
            dialogService = new DefaultDialogService();
            ColumnNumberText = "";
            this.MenuItems = new ObservableCollection<System.Windows.Controls.MenuItem>(RecentFiles.KeysCollectionShared.ShortenedNamesMenuItems);
            this.MenuItems.CollectionChanged += (s, e) => { SetCommands(); OnPropertyChanged(nameof(MenuItems)); };
            RecentFiles.KeysCollectionShared.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == RecentFiles.shortenedNamesMenuItemsPropertyName)
                {
                    this.MenuItems.Clear();
                    foreach (var el in RecentFiles.KeysCollectionShared.ShortenedNamesMenuItems)
                    {
                        MenuItems.Add(el);
                    }
                    //MenuItems.AddRange(RecentFiles.KeysCollectionShared.ShortenedNamesMenuItems);
                    OnPropertyChanged(nameof(MenuItems));
                    //MenuItems = RecentFiles.KeysCollectionShared.ShortenedNamesMenuItems;
                }
            };
        }

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
