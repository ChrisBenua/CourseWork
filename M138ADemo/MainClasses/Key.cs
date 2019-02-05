﻿using System;
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
    [Serializable()]
    [XmlType("KeyModel")]
    public class KeyModel : INotifyPropertyChanged
    {

       /* public override void ReadXml(XmlReader reader)
        {
            reader.Read();
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == GetType().ToString())
            {
                _key = reader["_key"];
                _idNumber = int.Parse(reader["_idNumber"]);
                _shift = int.Parse(reader["_shift"]);
                CopyToArr();
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(GetType().ToString());
            writer.WriteAttributeString("_key", _key);
            writer.WriteAttributeString("_idNumber", _idNumber.ToString());
            writer.WriteAttributeString("_shift", _shift.ToString());
            writer.WriteEndElement();
        }
        */
        [XmlElement("_key")]
        public string _key;
        [XmlElement("_idNumber")]
        public int _idNumber;
        [XmlElement("_shift")]
        public int _shift;
        [XmlIgnore]
        private string[] _keyarr;

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
        [XmlIgnore]
        public string StrIdNumber
        {
            get { return _idNumber.ToString(); }
        }

        [XmlIgnore]
        public int LastIndex
        {
            get
            {
                return (_key.Length - 1 + _shift);
            }
        }

        [XmlIgnore]
        public string[] KeyArr
        {
            get
            {
                if (_shift >= 0)
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
                else
                {
                    List<string> Idnum = new List<string>();
                    Idnum.Add(StrIdNumber);
                    var keyarr = Helper.Slice(_keyarr, 0, _keyarr.Length);

                    var temp = Helper.Concat(Idnum, keyarr);
                    var fSlice = Helper.Slice(temp.ToArray(), -_shift, _keyarr.Length + _shift + 1);
                    fSlice = Helper.Concat(fSlice, keyarr);
                    
                    List<string> right = Helper.CreateList<string>(-_shift, " ");
                    fSlice = Helper.Concat(fSlice, right);
                    return fSlice.ToArray();
                }
            }
            set
            {
                _keyarr = value;
                NotifyPropertyChanged();
            }
        }

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

        public KeyModel()
        {
            _key = "";
            _keyarr = new string[0];
            _shift = 0;
            _idNumber = 0;
        }

        public KeyModel(string key, int id)
        {
            _key = key;
            _keyarr = new string[key.Length];
            CopyToArr();
            _idNumber = id;
            _shift = 0;
        }

        public KeyModel(string key, int id, int shift) : this(key, id)
        {
            _key = key;
            _keyarr = new string[key.Length];
            CopyToArr();
            _idNumber = id;
            _shift = shift;
        }

        public string GetCurrentChar(int ind)
        {
            if (ind + _shift < 0)
            {
                throw new ConfigurationException("Ind + shift < 0 в KeyModel.GetCurrentChar");
            }
            return new string(_key[(ind + _shift) % 26], 1);
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void AdjustShiftEncrypt(char ch)
        {
            int ind = _key.IndexOf(ch);
            this.Shift = -ind;
        }

        public void AdjustShiftDecrypt(char ch)
        {
            int ind = _key.IndexOf(ch);
            int sh = Configuration.DecryptIndex - ind - 1;
            if (sh > 0)
            {
                sh -= 26;
            }
            this.Shift = sh;
        }

       /* public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            //reader.Read();
            //if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == GetType().ToString())
            {
                _key = reader["_key"];
                _idNumber = int.Parse(reader["_idNumber"]);
                _shift = int.Parse(reader["_shift"]);
                CopyToArr();
            }//
            reader.Read();
            reader.Read();

            _key = reader.Value;
            reader.Read();
            reader.Read();
            reader.Read();

            _idNumber = int.Parse(reader.Value);
            reader.Read();
            reader.Read();
            reader.Read();

            _shift = int.Parse(reader.Value);
            CopyToArr();
            reader.Read();
            reader.Read();
        }

        public void WriteXml(XmlWriter writer)
        {
            //writer.WriteElementString("_key", _key);
            //writer.WriteStartElement(GetType().ToString());
            writer.WriteElementString("_key", _key);
            writer.WriteElementString("_idNumber", _idNumber.ToString());
            writer.WriteElementString("_shift", _shift.ToString());
           // writer.WriteEndElement();
        }
    */
        [field: NonSerialized()]
        public event PropertyChangedEventHandler PropertyChanged;
    }

    [Serializable()]
    [XmlType("DeviceState")]
    public class DeviceState
    {
        [XmlArray]
        public ObservableCollection<KeyModel> keys;

        public DeviceState()
        {
            keys = new ObservableCollection<KeyModel>();
        }

        public DeviceState(List<KeyModel> lst)
        {
            keys = new ObservableCollection<KeyModel>(lst);
        }

        public DeviceState(ObservableCollection<KeyModel> lst)
        {
            keys = new ObservableCollection<KeyModel>(lst);
        }

        public DeviceState(BindingList<KeyModel> lst)
        {
            keys = new ObservableCollection<KeyModel>(lst);
        }

        public void AddKey(KeyModel k)
        {
            keys.Add(k);
        }

        public string GetCurrentChar(int row, int ind)
        {
            if (ind + keys[row].Shift < 0)
            {
                throw new ConfigurationException("Ind + shift < 0 в KeyModel.GetCurrentChar");
            }
            return keys[row].GetCurrentChar(ind);
        }
    }
}
