using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace M138ADemo.Models
{
    /// <summary>
    /// Main settings model.
    /// </summary>
    public class MainSettingsModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The message.
        /// </summary>
        string _message;

        /// <summary>
        /// The encrypt flag.
        /// </summary>
        private bool _encrypt = true;

        /// <summary>
        /// The automatic flag.
        /// </summary>
        private bool _automatic = true;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:M138ADemo.Models.MainSettingsModel"/> is automatic.
        /// </summary>
        /// <value><c>true</c> if automatic; otherwise, <c>false</c>.</value>
        public bool Automatic
        {
            get => _automatic;

            set
            {
                _automatic = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:M138ADemo.Models.MainSettingsModel"/> is encrypt.
        /// </summary>
        /// <value><c>true</c> if encrypt; otherwise, <c>false</c>.</value>
        public bool Encrypt
        {
            get => _encrypt;

            set
            {
                _encrypt = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message
        {
            get => _message;
            set
            {
                _message = Validate(value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Validate the specified s.
        /// </summary>
        /// <returns>The validate.</returns>
        /// <param name="s">S.</param>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.Models.MainSettingsModel"/> class.
        /// </summary>
        public MainSettingsModel()
        {
            Message = Configuration.Message ?? "";
        }

        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Ons the property changed.
        /// </summary>
        /// <param name="prop">Property.</param>
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
