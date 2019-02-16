using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M138ADemo.MainClasses;

namespace M138ADemo.ViewModels
{
    public class ShowKeysViewModel : AddUsersKeysViewModel
    {
        public ShowKeysViewModel()
        {
            foreach (var el in Configuration.lst)
            {
                this.UserKeys.Add(new KeyForPersistance(el.first, el.second));
            }
        }

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

                    Configuration.lst.Clear();
                    UserKeys.ToList().ForEach(el => Configuration.lst.Add(Pair<int, string>.MakePair(el.Id,
                        el.KeyArr.Aggregate((a, b) => a + b))));
                    this.OnNotifyOnClose();
                }));
            }
        }
    }
}
