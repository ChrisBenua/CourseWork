using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using M138ADemo.MainClasses;
using M138ADemo.Services;

namespace M138ADemo.ViewModels
{
    /// <summary>
    /// Drag and drop view model.
    /// </summary>
    public class DragAndDropViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The dialog service.
        /// </summary>
        private IDialogService dialogService;

        /// <summary>
        /// Key shift enum.
        /// </summary>
        public enum KeyShiftEnum
        {
            Left, Right
        }

        /// <summary>
        /// Windows to be opened.
        /// </summary>
        public enum WindowsToBeOpened
        {
            MainSettings, AddKeys
        }

        /// <summary>
        /// The title.
        /// </summary>
        string _title;

        /// <summary>
        /// The should force close.
        /// </summary>
        private bool _shouldForceClose = false;

        /// <summary>
        /// The keys.
        /// </summary>
        ObservableCollection<KeyModel> _keys;

        /// <summary>
        /// The last state.
        /// </summary>
        DeviceState _lastState;

        /// <summary>
        /// The selected column.
        /// </summary>
        int _selectedColumn;

        /// <summary>
        /// Gets or sets the selected column.
        /// </summary>
        /// <value>The selected column.</value>
        public int SelectedColumn
        {
            get => _selectedColumn;

            set
            {
                _selectedColumn = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get => _title;

            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the last state.
        /// </summary>
        /// <value>The last state.</value>
        public DeviceState LastState
        {
            get
            {
                return _lastState;
            }

            private set
            {
                _lastState = value;
            }
        }

        /// <summary>
        /// Gets or sets the keys.
        /// </summary>
        /// <value>The keys.</value>
        public ObservableCollection<KeyModel> Keys
        {
            get
            {
                return _keys;
            }
            set
            {
                _keys = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The shift command.
        /// </summary>
        private RelayCommand _shiftCommand;

        /// <summary>
        /// Gets the shift command.
        /// </summary>
        /// <value>The shift command.</value>
        public RelayCommand ShiftCommand
        {
            get
            {
                return _shiftCommand ?? (_shiftCommand = new RelayCommand(obj =>
                {
                    (int, KeyShiftEnum) convObj = ((int, KeyShiftEnum))obj;
                    KeyShiftEnum choice = convObj.Item2;
                    int index = convObj.Item1;
                    if (choice == KeyShiftEnum.Left)
                    {
                        if (_keys[index].Shift > -26)
                            _keys[index].Shift--;
                    }
                    else
                    {
                        if (_keys[index].Shift < 0)
                            _keys[index].Shift++;
                    }

                    UpdateAndRehighlight?.Invoke(index);
                }));
            }
        }

        /// <summary>
        /// The on show selected text.
        /// </summary>
        private RelayCommand _onShowSelectedText;

        /// <summary>
        /// Gets the on show selected text.
        /// </summary>
        /// <value>The on show selected text.</value>
        public RelayCommand OnShowSelectedText
        {
            get
            {
                
                return _onShowSelectedText ?? (_onShowSelectedText = new RelayCommand(obj => {
                    string columnText = "";

                    for (int i = 0; i < Keys.Count; ++i)
                    {
                        columnText += Keys[i].KeyArr[SelectedColumn];
                    }
                    Clipboard.SetText(columnText);

                    dialogService.ShowMessage(columnText, "Ваше сообщение, скопировано в буфер обмена");
                }));
            }
        }

        /// <summary>
        /// The on closing command.
        /// </summary>
        private RelayCommand _onClosingCommand;

        /// <summary>
        /// Gets the on closing command.
        /// </summary>
        /// <value>The on closing command.</value>
        public RelayCommand OnClosingCommand
        {
            get
            {
                return _onClosingCommand ?? (_onClosingCommand = new RelayCommand(obj =>
                {
                    var e = (CancelEventArgs)obj;
                    if ((this.Title == "Untitled" || DidUserMadeChanges) && !_shouldForceClose)
                    {
                        var res = MessageBox.Show("Вы не сохранили файл, все данные будут утерены, если вы решите выйти", "Предупреждение", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                        e.Cancel = res != System.Windows.Forms.DialogResult.OK;
                    }
                }));
            }
        }

        /// <summary>
        /// The save as command.
        /// </summary>
        private RelayCommand _saveAsCommand;

        /// <summary>
        /// Gets the save as command.
        /// </summary>
        /// <value>The save as command.</value>
        public RelayCommand SaveAsCommand
        {
            get
            {
                return _saveAsCommand ?? (_saveAsCommand = new RelayCommand(obj =>
                {
                    if (dialogService.SaveFileDialog())
                    {
                        string fileName = dialogService.FilePath;
                        Title = fileName;

                        var savedState = IOHelper.SaveDeviceState(fileName, new List<KeyModel>(Keys));
                        if (savedState != null)
                        {
                            LastState.SafeInit(savedState.keys);
                        }
                    }

                }));
            }
        }

        /// <summary>
        /// The open menu item command.
        /// </summary>
        private RelayCommand _openMenuItemCommand;

        /// <summary>
        /// Gets the open menu item command.
        /// </summary>
        /// <value>The open menu item command.</value>
        public RelayCommand OpenMenuItemCommand
        {
            get
            {
                return _openMenuItemCommand ?? (_openMenuItemCommand = new RelayCommand(obj =>
                {
                    if (dialogService.OpenFileDialog())
                    {
                        string fileName = dialogService.FilePath;

                        DeviceState state = null;
                        string newWindowTitle = null;

                        (state, newWindowTitle) = IOHelper.LoadDeviceState(fileName);

                        Title = newWindowTitle;

                        if (state != null)
                        {
                            Keys.Clear();

                            foreach (var el in state.keys)
                            {
                                el.CopyToArr();
                                Keys.Add(el);
                            }
                            LastState.SafeInit(state.keys);
                            Configuration.DeviceState = state;

                            AsyncUtils.DelayCall(1000, AfterFileOpened);
                        }
                    }
                }));
            }
        }
        /// <summary>
        /// The is back to keys menu item enabled.
        /// </summary>
        bool _isBackToKeysMenuItemEnabled;

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:M138ADemo.ViewModels.DragAndDropViewModel"/> is back to
        /// keys menu items enabled.
        /// </summary>
        /// <value><c>true</c> if is back to keys menu items enabled; otherwise, <c>false</c>.</value>
        public bool IsBackToKeysMenuItemsEnabled
        {
            get
            {
                return _isBackToKeysMenuItemEnabled;
            }

            private set
            {
                _isBackToKeysMenuItemEnabled = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The back command.
        /// </summary>
        private RelayCommand _backCommand;

        /// <summary>
        /// Gets the back command.
        /// </summary>
        /// <value>The back command.</value>
        public RelayCommand BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new RelayCommand(obj =>
                {
                    var choice = (WindowsToBeOpened)obj;

                    switch (choice)
                    {
                        case WindowsToBeOpened.AddKeys:
                            _shouldForceClose = true;
                            if (Configuration.Encrypt)
                            {
                                AddKeys w = new AddKeys();
                                w.Show();
                            }
                            else
                            {
                                DecryptAddKeys w1 = new DecryptAddKeys();
                                w1.Show();
                            }
                            NotifyToClose?.Invoke();
                            break;
                        case WindowsToBeOpened.MainSettings:
                            _shouldForceClose = true;
                            MainSettings w2 = new MainSettings();
                            w2.Show();
                            NotifyToClose?.Invoke();

                            break;

                    }

                }));
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:M138ADemo.ViewModels.DragAndDropViewModel"/> did user made changes.
        /// </summary>
        /// <value><c>true</c> if did user made changes; otherwise, <c>false</c>.</value>
        public bool DidUserMadeChanges
        {
            get
            {
                if (LastState.keys.Count == Keys.Count)
                {
                    foreach (var el in LastState.keys)
                    {
                        if ((Keys.Where((arg) => arg.IdNumber == el.IdNumber && arg.Key == el.Key && arg.Shift == el.Shift)).Count() != (LastState.keys.Where((arg) => arg.IdNumber == el.IdNumber && arg.Key == el.Key && arg.Shift == el.Shift)).Count())
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.ViewModels.DragAndDropViewModel"/> class.
        /// </summary>
        public DragAndDropViewModel()
        {
            Keys = new ObservableCollection<KeyModel>();
            LastState = new DeviceState();
            dialogService = new DefaultDialogService();
            if (Configuration.DeviceState != null)
            {
                Configuration.DeviceState.keys.ToList().ForEach((arg) => Keys.Add(new KeyModel(arg._key, arg.IdNumber, arg.Shift)));
                LastState.SafeInit(Configuration.DeviceState.keys);
            }
            else
            {
                Configuration.KeyList.ToList().ForEach((arg) => Keys.Add(new KeyModel(arg.Item2, arg.Item1)));
                LastState.SafeInit(Keys);
                Title = "Untitled";
            }
            AdjustShifts();

            IsBackToKeysMenuItemsEnabled = Configuration.Message == null ? false : true;
        }

        /// <summary>
        /// Ons the property changed.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        public void OnPropertyChanged([CallerMemberName]string propertyName="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName: propertyName));
        }

        /// <summary>
        /// Adjusts the shifts.
        /// </summary>
        public void AdjustShifts()
        {
            if (Configuration.Automatic && Configuration.Encrypt)
            {
                string mes = Configuration.Message ?? "";
                for (int i = 0; i < Math.Min(mes.Length, this.Keys.Count); ++i)
                {
                    Keys[i].AdjustShiftEncrypt(Char.ToLower(mes[i]));
                }
            }
            if (Configuration.Automatic && Configuration.Decrypt)
            {
                string mes = Configuration.Message ?? "";
                for (int i = 0; i < Math.Min(mes.Length, this.Keys.Count); ++i)
                {
                    Keys[i].AdjustShiftDecrypt(Char.ToLower(mes[i]));
                }
            }
        }

        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when update and rehighlight.
        /// </summary>
        public event Action<int> UpdateAndRehighlight;

        /// <summary>
        /// Occurs when after file opened.
        /// </summary>
        public event Action AfterFileOpened;

        /// <summary>
        /// Occurs when notify to close.
        /// </summary>
        public event Action NotifyToClose;
    }
}
