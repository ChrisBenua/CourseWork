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
    public class DragAndDropViewModel : INotifyPropertyChanged
    {

        private IDialogService dialogService;

        public enum KeyShiftEnum
        {
            Left, Right
        }

        string _title;

        ObservableCollection<KeyModel> _keys;

        DeviceState _lastState;

        public string Title
        {
            get => _title;

            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

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

        private RelayCommand _shiftCommand;

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

        

        private RelayCommand _onClosingCommand;

        public RelayCommand OnClosingCommand
        {
            get
            {
                return _onClosingCommand ?? (_onClosingCommand = new RelayCommand(obj =>
                {
                    var e = (CancelEventArgs)obj;
                    if (this.Title == "Untitled" || DidUserMadeChanges)
                    {
                        var res = MessageBox.Show("Вы не сохранили файл, все данные будут утерены, если вы решите выйти", "Предупреждение", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                        e.Cancel = res != System.Windows.Forms.DialogResult.OK;
                    }
                }));
            }
        }

        private RelayCommand _saveAsCommand;

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

        private RelayCommand _openMenuItemCommand;

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
                            Configuration.deviceState = state;

                            AsyncUtils.DelayCall(1000, AfterFileOpened);
                        }
                    }
                }));
            }
        }

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

        public DragAndDropViewModel()
        {
            Keys = new ObservableCollection<KeyModel>();
            LastState = new DeviceState();

            if (Configuration.deviceState != null)
            {
                Configuration.deviceState.keys.ToList().ForEach((arg) => Keys.Add(new KeyModel(arg._key, arg.IdNumber, arg.Shift)));
                LastState.SafeInit(Configuration.deviceState.keys);
            }
            else
            {
                Configuration.lst.ToList().ForEach((arg) => Keys.Add(new KeyModel(arg.second, arg.first)));
                LastState.SafeInit(Keys);
            }
            AdjustShifts();
        }

        public void OnPropertyChanged([CallerMemberName]string propertyName="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName: propertyName));
        }


        public void AdjustShifts()
        {
            if (Configuration.Automatic && Configuration.Encrypt)
            {
                string mes = Configuration.Message;
                for (int i = 0; i < mes.Length; ++i)
                {
                    Keys[i].AdjustShiftEncrypt(Char.ToLower(mes[i]));
                }
            }
            if (Configuration.Automatic && Configuration.Decrypt)
            {
                string mes = Configuration.Message;
                for (int i = 0; i < mes.Length; ++i)
                {
                    Keys[i].AdjustShiftDecrypt(Char.ToLower(mes[i]));
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public event Action<int> UpdateAndRehighlight;

        public event Action AfterFileOpened;
    }
}
