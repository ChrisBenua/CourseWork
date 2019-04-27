using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M138ADemo.MainClasses;

namespace M138ADemo.ViewModels
{
    /// <summary>
    /// Show keys view model.
    /// </summary>
    public class ShowKeysViewModel : AddUsersKeysViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.ViewModels.ShowKeysViewModel"/> class.
        /// </summary>
        public ShowKeysViewModel()
        {
            foreach (var el in Configuration.KeyList)
            {
                this.UserKeys.Add(new KeyForPersistance(el.Item1, el.Item2));
            }
        }

        /// <summary>
        /// Gets the check keys identifiers command.
        /// </summary>
        /// <value>The check keys identifiers command.</value>
        public override RelayCommand CheckKeysIdsCommand
        {
            get
            {
                return _checkKeysIdsCommand ?? (_checkKeysIdsCommand = new RelayCommand(obj =>
                {
                    HashSet<int> set = new HashSet<int>();
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

        /// <summary>
        /// Gets the end command.
        /// </summary>
        /// <value>The end command.</value>
        public override RelayCommand EndCommand
        {
            get
            {
                return _endCommand ?? (_endCommand = new RelayCommand(obj =>
                {
                    HashSet<int> set = new HashSet<int>();

                    for (int i = 0; i < UserKeys.Count; ++i)
                    {
                        if (set.Contains(UserKeys[i].Id) || UserKeys.Any(arg => arg.KeyArr.Any(el => el.Length == 0)))
                        {
                            dialogService.ShowMessage("Повторяющиеся Id у ключей, невозможно сохранить или не все ключи заполнены полностью", "Ошибка");
                            return;
                        }
                        set.Add(UserKeys[i].Id);
                    }

                    Configuration.KeyList.Clear();
                    UserKeys.ToList().ForEach(el => Configuration.KeyList.Add((el.Id,
                        el.KeyArr.Aggregate((a, b) => a + b))));
                    this.OnNotifyOnClose();
                }));
            }
        }
    }
}
