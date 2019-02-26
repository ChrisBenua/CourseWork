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
    public class DecryptAddKeysViewModel: INotifyPropertyChanged
    {
        private IDialogService dialogService;

        private ObservableCollection<Pair<int, string>> _keys;

        public ObservableCollection<Pair<int, string>> Keys
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

        public int columnNumber = -1;

        private string _columnNumberText;

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

        private Brush _nextButtonColor;

        public Brush NextButtonColor
        {
            get => _nextButtonColor;

            set
            {
                _nextButtonColor = value;
                OnPropertyChanged();
            }
        }

        private bool _isNextButtonEnabled;

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

        private RelayCommand _openKeysFromFile;

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
                            this.Keys.Add(Pair<int, string>.MakePair(el.Id, el.Key));
                        }
                        PerformValidation();
                    }
                }));
            }
        }

        private RelayCommand _saveFileCommand;

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

        public DecryptAddKeysViewModel()
        {
            this.Keys = Configuration.lst;
            this.NextButtonColor = Brushes.LightGray;
            dialogService = new DefaultDialogService();
            ColumnNumberText = "";
        }

        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
