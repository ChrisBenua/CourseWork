using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;


namespace M138ADemo
{
    /// <summary>
    /// Key model.
    /// </summary>
    [Serializable()]
    [XmlType("KeyModel")]
    public class KeyModel : INotifyPropertyChanged
    {

        /// <summary>
        /// The key.
        /// </summary>
        [XmlElement("_key")]
        public string _key;

        /// <summary>
        /// The identifier number.
        /// </summary>
        [XmlElement("_idNumber")]
        public int _idNumber;

        /// <summary>
        /// The shift.
        /// </summary>
        [XmlElement("_shift")]
        public int _shift;

        /// <summary>
        /// The keyarr.
        /// </summary>
        [XmlIgnore]
        private string[] _keyarr;

        /// <summary>
        /// Copies to arr.
        /// </summary>
        public void CopyToArr()
        {
            if (_keyarr.Length != _key.Length)
            {
                _keyarr = new string[_key.Length];
            }
            for (int i = 0; i < _key.Length; ++i)
            {
                _keyarr[i] = new string(Helper.ToUpper(_key[i]), 1);
            }

            //_keyarr[_key.Length] = ' ';
        }

        /// <summary>
        /// Gets the string identifier number.
        /// </summary>
        /// <value>The string identifier number.</value>
        [XmlIgnore]
        public string StrIdNumber
        {
            get { return _idNumber.ToString(); }
        }

        /// <summary>
        /// Gets the last index.
        /// </summary>
        /// <value>The last index.</value>
        [XmlIgnore]
        public int LastIndex
        {
            get
            {
                return (_key.Length - 1 + _shift);
            }
        }

        /// <summary>
        /// Gets or sets the key arr.
        /// </summary>
        /// <value>The key arr.</value>
        [XmlIgnore]
        public string[] KeyArr
        {
            get
            {
                /*if (_shift >= 0)
                {

                    var fSlice = Helper.Slice(_keyarr, 0, _keyarr.Length - _shift);
                    var keyarr = Helper.Slice(_keyarr, 0, _keyarr.Length);
                    fSlice = Helper.Concat(keyarr, fSlice);
                    List<string> Idnum = new List<string>();
                    Idnum.Add(StrIdNumber);
                    var mainPart = Helper.Concat(Idnum, fSlice);
                    List<string> left = Helper.CreateList<string>(_shift, " ");
                    
                    mainPart = Helper.Concat<string>(left, mainPart);
                    
                    return mainPart.ToArray();
                }
                else*/
                {
                    if (Configuration.IsCompactWorkSpace)
                    {
                        List<string> Idnum = new List<string>();
                        Idnum.Add(StrIdNumber);
                        //var keyarr = Helper.Slice(_keyarr, 0, _keyarr.Length);

                        //var temp = Helper.Concat(Idnum, keyarr);
                        var temp = Idnum.Concat(_keyarr);
                        var fSlice = temp.Skip(-_shift).Take(_keyarr.Length + _shift + 1);
                        //var fSlice = Helper.Slice(temp.ToArray(), -_shift, _keyarr.Length + _shift + 1);
                        //fSlice = Helper.Concat(fSlice, keyarr);
                        fSlice = fSlice.Concat(_keyarr);
                        List<string> right = Helper.CreateList<string>(-_shift, " ");
                        //fSlice = Helper.Concat(fSlice, right);
                        fSlice = fSlice.Concat(right);
                        return fSlice.ToArray();
                    }
                    else
                    {
                        List<string> Idnum = new List<string>();
                        Idnum.Add(StrIdNumber);
                        //var keyarr = Helper.Slice(_keyarr, 0, _keyarr.Length);

                        //var temp = Helper.Concat(Idnum, keyarr);
                        var temp = Idnum.Concat(_keyarr);
                        //temp = Helper.Concat(temp, keyarr);
                        temp = temp.Concat(Helper.CreateList<string>(-_shift, " "));
                        //temp = Helper.Concat(temp, Helper.CreateList<string>(-_shift, " "));
                        var prefixArr = new List<string>();
                        for (int i = 0; i < 26 + Shift; ++i)
                        {
                            prefixArr.Add(" ");
                        }
                        var res = prefixArr.Concat(temp);
                        //var res = Helper.Concat(prefixArr, temp);
                        return res.ToArray();
                    }
                }
            }
            set
            {
                _keyarr = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        [XmlIgnore]
        public string Key
        {
            get
            {
                if (_shift > 0)
                {
                    return _key.Substring(_key.Length - _shift, _shift) + _key.Substring(0, _key.Length - _shift);
                }
                else
                {
                    return _key.Substring(-_shift, _key.Length + _shift) + _key.Substring(0, -_shift);
                }
            }
            set
            {
                if (value.Length != 26)
                {
                    throw new ConfigurationException("Присвоил KeyModel.Key строку неправильного размера");
                }

                _key = value;
                CopyToArr();
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the identifier number.
        /// </summary>
        /// <value>The identifier number.</value>
        [XmlIgnore]
        public int IdNumber
        {
            get
            {
                return _idNumber;
            }
            set
            {
                if (value < 0 || value > Helper.MaxKeys)
                {
                    throw new ConfigurationException("Присвоил KeyModel.IdNumber что-то < 0 или > MaxKeys");
                }

                _idNumber = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the shift.
        /// </summary>
        /// <value>The shift.</value>
        [XmlIgnore]
        public int Shift
        {
            get { return _shift; }
            set
            {
                /*if (value < -26 || value > 26)
                {
                    throw new ConfigurationException("Присвоил KeyModel.Shift либо <-26 либо >26");
                }*/

                _shift = value;
                //_shift %= 26;
                NotifyPropertyChanged("Shift");
                NotifyPropertyChanged("KeyArr");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.KeyModel"/> class.
        /// </summary>
        public KeyModel()
        {
            _key = "";
            _keyarr = new string[0];
            _shift = 0;
            _idNumber = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.KeyModel"/> class.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="id">Identifier.</param>
        public KeyModel(string key, int id)
        {
            _key = key;
            _keyarr = new string[key.Length];
            CopyToArr();
            _idNumber = id;
            _shift = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.KeyModel"/> class.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="id">Identifier.</param>
        /// <param name="shift">Shift.</param>
        public KeyModel(string key, int id, int shift)
        {
            _key = key;
            _keyarr = new string[key.Length];
            CopyToArr();
            _idNumber = id;
            _shift = shift;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.KeyModel"/> class.
        /// </summary>
        /// <param name="other">Other.</param>
        public KeyModel(KeyModel other)
        {
            _key = String.Copy(other._key);
            _keyarr = new string[_key.Length];
            CopyToArr();
            _idNumber = other._idNumber;
            _shift = other._shift;
        }

        /// <summary>
        /// Gets the current char.
        /// </summary>
        /// <returns>The current char.</returns>
        /// <param name="ind">Ind.</param>
        public string GetCurrentChar(int ind)
        {
            if (ind + _shift < 0)
            {
                throw new ConfigurationException("Ind + shift < 0 в KeyModel.GetCurrentChar");
            }
            return new string(_key[(ind + _shift) % 26], 1);
        }

        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Adjusts the shift encrypt.
        /// </summary>
        /// <param name="ch">Ch.</param>
        public void AdjustShiftEncrypt(char ch)
        {
            if (ch >= 'a' && ch <= 'z' && _key[0] >= 'A' && _key[0] <= 'Z')
            {
                ch = Helper.ToUpper(ch);
            }
            if (Char.IsUpper(ch) && !Char.IsUpper(_key[0]))
            //if (Helper.isUpperAlpha(ch) && !Helper.isUpperAlpha(_key[0]))
            {
                ch = Char.ToLower(ch);
            }
            int ind = _key.IndexOf(ch);
            this.Shift = -ind;
        }

        /// <summary>
        /// Adjusts the shift decrypt.
        /// </summary>
        /// <param name="ch">Ch.</param>
        public void AdjustShiftDecrypt(char ch)
        {
            if (ch >= 'a' && ch <= 'z' && _key[0] >= 'A' && _key[0] <= 'Z')
            {
                ch = Helper.ToUpper(ch);
            }
            if (Char.IsUpper(ch) && !Char.IsUpper(_key[0]))
            {
                ch = Char.ToLower(ch);
            }
            int ind = _key.IndexOf(ch);
            int sh = Configuration.DecryptIndex - ind - 1;
            if (sh > 0)
            {
                sh -= 26;
            }
            this.Shift = sh;
        }

        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        [field: NonSerialized()]
        public event PropertyChangedEventHandler PropertyChanged;
    }

    /// <summary>
    /// Device state.
    /// </summary>
    [Serializable()]
    [XmlType("DeviceState")]
    public class DeviceState
    {
        /// <summary>
        /// The keys.
        /// </summary>
        [XmlArray]
        public ObservableCollection<KeyModel> keys;

        /// <summary>
        /// Safe init.
        /// </summary>
        /// <param name="obs">Obs.</param>
        public void SafeInit(ObservableCollection<KeyModel> obs)
        {
            keys = new ObservableCollection<KeyModel>();
            foreach (var el in obs)
            {
                keys.Add(new KeyModel(el));
            }
        }

        /// <summary>
        /// Safe init.
        /// </summary>
        /// <param name="obs">Obs.</param>
        public void SafeInit(BindingList<KeyModel> obs)
        {
            keys = new ObservableCollection<KeyModel>();
            foreach (var el in obs)
            {
                keys.Add(new KeyModel(el));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.DeviceState"/> class.
        /// </summary>
        public DeviceState()
        {
            keys = new ObservableCollection<KeyModel>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.DeviceState"/> class.
        /// </summary>
        /// <param name="lst">Lst.</param>
        public DeviceState(IEnumerable<KeyModel> lst)
        {
            keys = new ObservableCollection<KeyModel>(lst);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.DeviceState"/> class.
        /// </summary>
        /// <param name="lst">Lst.</param>
        public DeviceState(BindingList<KeyModel> lst)
        {
            keys = new ObservableCollection<KeyModel>(lst);
        }

        /// <summary>
        /// Adds the key.
        /// </summary>
        /// <param name="k">K.</param>
        public void AddKey(KeyModel k)
        {
            keys.Add(k);
        }

        /// <summary>
        /// Gets the current char.
        /// </summary>
        /// <returns>The current char.</returns>
        /// <param name="row">Row.</param>
        /// <param name="ind">Ind.</param>
        public string GetCurrentChar(int row, int ind)
        {
            if (ind + keys[row].Shift < 0)
            {
                throw new ConfigurationException("Ind + shift < 0 в KeyModel.GetCurrentChar");
            }
            return keys[row].GetCurrentChar(ind);
        }
    }

    /// <summary>
    /// Key for persistance.
    /// </summary>
    [XmlType("KeyForPersistance")]
    public class KeyForPersistance
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [XmlElement("Id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        [XmlElement("Key")]
        public string Key { get; set; }

        /// <summary>
        /// The key arr.
        /// </summary>
        private string[] keyArr;

        /// <summary>
        /// Makes the key arr.
        /// </summary>
        private void makeKeyArr()
        {
            if (Key.Length == 0)
            {
                KeyArr = new string[26];
                for (int i = 0; i < KeyArr  .Length; ++i)
                {
                    KeyArr[i] = "";
                }
            }
            else
            {
                KeyArr = new string[Key.Length];
                for (int i = 0; i < Key.Length; ++i)
                {
                    KeyArr[i] = new string(Key[i], 1);
                }
            }
        }

        /// <summary>
        /// Gets or sets the key arr.
        /// </summary>
        /// <value>The key arr.</value>
        [XmlIgnore]
        public string[] KeyArr
        {
            get
            {
                return keyArr;
            }

            set
            {
                keyArr = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.KeyForPersistance"/> class.
        /// </summary>
        public KeyForPersistance()
        {
            Id = 0;
            Key = "";
            makeKeyArr();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.KeyForPersistance"/> class.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="key">Key.</param>
        public KeyForPersistance(int id, string key)
        {
            Id = id;
            Key = key;
            makeKeyArr();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.KeyForPersistance"/> class.
        /// </summary>
        /// <param name="p">P.</param>
        public KeyForPersistance((int, String) p)
        {
            Id = p.Item1;
            Key = p.Item2;
            makeKeyArr();
        }
    }

    /// <summary>
    /// Keys container.
    /// </summary>
    [XmlType("KeysContainer")]
    public class KeysContainer
    {
        /// <summary>
        /// The keys.
        /// </summary>
        [XmlElement("keys")]
        public ObservableCollection<KeyForPersistance> keys;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.KeysContainer"/> class.
        /// </summary>
        public KeysContainer()
        {
            keys = new ObservableCollection<KeyForPersistance>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.KeysContainer"/> class.
        /// </summary>
        /// <param name="keys">Keys.</param>
        public KeysContainer(ObservableCollection<(int, String)> keys)
        {
            this.keys = new ObservableCollection<KeyForPersistance>();
            foreach (var el in keys)
            {
                this.keys.Add(new KeyForPersistance(el));
            }
        }

    }
}
