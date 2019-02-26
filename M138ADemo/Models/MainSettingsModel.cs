using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace M138ADemo.Models
{
    public class MainSettingsModel : INotifyPropertyChanged
    {
        string _message;
        private bool _encrypt = true;

        private bool _automatic = true;

        public bool Automatic
        {
            get => _automatic;

            set
            {
                _automatic = value;
                OnPropertyChanged();
            }
        }

        public bool Encrypt
        {
            get => _encrypt;

            set
            {
                _encrypt = value;
                OnPropertyChanged();
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                _message = Validate(value);
                OnPropertyChanged();
            }
        }

        private string Validate(string s)
        {
            string ans = s.Replace(" ", "");
            ans = new String((from ch in ans.ToCharArray() where (ch <= 'Z' && ch >= 'A') select ch).ToArray());
            if (ans.Length > 100)
            {
               ans = ans.Substring(0, 100);
            }
            return ans;
        }

        public MainSettingsModel()
        {
            Message = Configuration.Message ?? "";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
