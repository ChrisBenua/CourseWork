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
    /// <summary>
    /// Add users keys view model.
    /// </summary>
    public class AddUsersKeysViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The dialog service.
        /// </summary>
        protected IDialogService dialogService = new DefaultDialogService();

        /// <summary>
        /// The user keys.
        /// </summary>
        protected ObservableCollection<KeyForPersistance> _userKeys;

        /// <summary>
        /// Gets or sets the user keys.
        /// </summary>
        /// <value>The user keys.</value>
        public ObservableCollection<KeyForPersistance> UserKeys
        {
            get => _userKeys;

            set
            {
                _userKeys = value;
                
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The delete row command.
        /// </summary>
        protected RelayCommand _deleteRowCommand;

        /// <summary>
        /// Gets the delete row command.
        /// </summary>
        /// <value>The delete row command.</value>
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

        /// <summary>
        /// The add key command.
        /// </summary>
        protected RelayCommand _addKeyCommand;

        /// <summary>
        /// Gets the addkey command.
        /// </summary>
        /// <value>The addkey command.</value>
        public RelayCommand AddkeyCommand
        {
            get
            {
                return _addKeyCommand ?? (_addKeyCommand = new RelayCommand(obj =>
                {
                    UserKeys.Add(new KeyForPersistance(0, ""));
                }));
            }
        }

        /// <summary>
        /// The end command.
        /// </summary>
        protected RelayCommand _endCommand;

        /// <summary>
        /// Gets the end command.
        /// </summary>
        /// <value>The end command.</value>
        public virtual RelayCommand EndCommand
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

                    UserKeys.ToList().ForEach(arg => Configuration.KeyList.Add((arg.Id, 
                        arg.KeyArr.Aggregate((a, b) => a + b))));
                    NotifyOnClose?.Invoke();

                }));
            }
        }

        /// <summary>
        /// The key preview input command.
        /// </summary>
        protected RelayCommand _keyPreviewInputCommand;

        /// <summary>
        /// Gets the key preview input command.
        /// </summary>
        /// <value>The key preview input command.</value>
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

        /// <summary>
        /// The check keys identifiers command.
        /// </summary>
        protected RelayCommand _checkKeysIdsCommand;

        /// <summary>
        /// Gets the check keys identifiers command.
        /// </summary>
        /// <value>The check keys identifiers command.</value>
        public virtual RelayCommand CheckKeysIdsCommand
        {
            get
            {
                return _checkKeysIdsCommand ?? (_checkKeysIdsCommand = new RelayCommand(obj =>
                {
                    HashSet<int> set = new HashSet<int>(Configuration.forbiddenKeys);
                    for (int i = 0; i < UserKeys.Count; ++i)
                    {
                        if (set.Contains(UserKeys[i].Id))
                        {
                            dialogService.ShowMessage("Повторяющиеся номера ключей", "Предупреждение");
                            return;
                        }
                        set.Add(UserKeys[i].Id);
                    }
                }));
            }
        }

        /// <summary>
        /// Ises the sequence.
        /// </summary>
        /// <returns><c>true</c>, if sequence was ised, <c>false</c> otherwise.</returns>
        /// <param name="row">Row.</param>
        /// <param name="newText">New text.</param>
        public bool isSequence(int row, string newText)
        {
            HashSet<char> set = new HashSet<char>();
            set.Add(Char.ToLower(newText[0]));
            for (int i = 0; i < UserKeys[row].KeyArr.Length; ++i)
            {
                if (UserKeys[row].KeyArr[i].Length > 0)
                {
                    char str = Char.ToLower(UserKeys[row].KeyArr[i][0]);
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

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.ViewModels.AddUsersKeysViewModel"/> class.
        /// </summary>
        public AddUsersKeysViewModel()
        {
            UserKeys = new ObservableCollection<KeyForPersistance>();
            //UserKeys.Add(new KeyForPersistance(11, "abcdefghijklmnopqrstuvwxyz"));
        }

        /// <summary>
        /// Ons the property changed.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        public void OnPropertyChanged([CallerMemberName]string propertyName="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Ons the notify on close.
        /// </summary>
        protected void OnNotifyOnClose()
        {
            this.NotifyOnClose?.Invoke();
        }

        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when notify on close.
        /// </summary>
        public event Action NotifyOnClose;
    }
}
