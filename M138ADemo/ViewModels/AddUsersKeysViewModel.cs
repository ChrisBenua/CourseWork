using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using M138ADemo.MainClasses;
using System.Threading.Tasks;
using System.Collections.Generic;
using M138ADemo.Services;

namespace M138ADemo.ViewModels
{
    public class AddUsersKeysViewModel : INotifyPropertyChanged
    {
        private IDialogService dialogService = new DefaultDialogService();

        private ObservableCollection<KeyForPersistance> _userKeys;

        public ObservableCollection<KeyForPersistance> UserKeys
        {
            get => _userKeys;

            set
            {
                _userKeys = value;
                
                OnPropertyChanged();
            }
        }

        private RelayCommand _deleteRowCommand;

        public RelayCommand DeleteRowCommand
        {
            get
            {
                return _deleteRowCommand ?? (_deleteRowCommand = new RelayCommand(obj =>
                {
                    int index = (int)obj;

                    UserKeys.RemoveAt(index);
                }));
            }
        }

        private RelayCommand _addKeyCommand;

        public RelayCommand AddkeyCommand
        {
            get
            {
                return _addKeyCommand ?? (_addKeyCommand = new RelayCommand(obj =>
                {
                    UserKeys.Add(new KeyForPersistance(11, "abcdefghijklmnopqrstuvwxyz"));
                }));
            }
        }

        private RelayCommand _endCommand;

        public RelayCommand EndCommand
        {
            get
            {
                return _endCommand ?? (_endCommand = new RelayCommand(obj =>
                {
                    HashSet<int> set = new HashSet<int>(Configuration.forbiddenKeys);
                    bool flagOk = true;
                    for (int i = 0; i < UserKeys.Count; ++i)
                    {
                        if (set.Contains(UserKeys[i].Id) || UserKeys.Any(arg => arg.KeyArr.Any(el => el.Length == 0)))
                        {
                            dialogService.ShowMessage("Повторяющиеся Id у ключей, невозможно сохранить или не все ключи заполнены полностью", "Ошибка");
                            flagOk = false;
                            return;
                        }
                        set.Add(UserKeys[i].Id);
                    }

                    UserKeys.ToList().ForEach(arg => Configuration.lst.Add(Pair<int, string>.MakePair(arg.Id, 
                        arg.KeyArr.Aggregate((a, b) => a + b))));
                    NotifyOnClose?.Invoke();

                }));
            }
        }

        public RelayCommand _keyPreviewInputCommand;

        public RelayCommand KeyPreviewInputCommand
        {
            get
            {
                return _keyPreviewInputCommand ?? (_keyPreviewInputCommand = new RelayCommand(obj =>
                {
                    (string, System.Windows.Input.TextCompositionEventArgs) tuple = ((string, System.Windows.Input.TextCompositionEventArgs))obj;
                    string text = tuple.Item1;
                    var e = tuple.Item2;
                    int temp = 0;
                    if (Helper.reg.IsMatch(text) && int.TryParse(text, out temp) && temp <= Helper.MaxKeys)
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        e.Handled = true;

                        dialogService.ShowMessage("Тут не число или оно слишком большое", "Ошибка");
                    }


                }));
            }
        }

        private RelayCommand _checkKeysIdsCommand;

        public RelayCommand CheckKeysIdsCommand
        {
            get
            {
                return _checkKeysIdsCommand ?? (_checkKeysIdsCommand = new RelayCommand(obj =>
                {
                    HashSet<int> set = new HashSet<int>(Configuration.forbiddenKeys);
                    bool flagOk = true;
                    for (int i = 0; i < UserKeys.Count; ++i)
                    {
                        if (set.Contains(UserKeys[i].Id))
                        {
                            dialogService.ShowMessage("Повторяющиеся номера ключей", "Предупреждение");
                            flagOk = false;
                            return;
                        }
                        set.Add(UserKeys[i].Id);
                    }
                }));
            }
        }

        public bool isSequence(int row, string newText)
        {
            HashSet<char> set = new HashSet<char>();
            set.Add(newText[0]);
            for (int i = 0; i < UserKeys[row].KeyArr.Length; ++i)
            {
                if (UserKeys[row].KeyArr[i].Length > 0)
                {
                    char str = UserKeys[row].KeyArr[i][0];
                    if (str != ' ')
                    {
                        if (set.Contains(str))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public AddUsersKeysViewModel()
        {
            UserKeys = new ObservableCollection<KeyForPersistance>();
            UserKeys.Add(new KeyForPersistance(11, "abcdefghijklmnopqrstuvwxyz"));
        }

        public void OnPropertyChanged([CallerMemberName]string propertyName="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event Action NotifyOnClose;
    }
}
