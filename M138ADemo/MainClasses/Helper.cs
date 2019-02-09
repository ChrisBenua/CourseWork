using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using M138ADemo.MainClasses;

namespace M138ADemo
{

    public static class IOHelper
    {

        public static KeysContainer LoadKeysContainer(string fileName)
        {
            RecentFiles.KeysCollectionShared.AddFileToRecents(fileName);

            //CurrentFilePath = fileName;

            KeysContainer state = new KeysContainer();

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(state.GetType());

            System.IO.StreamReader reader = new System.IO.StreamReader(fileName);
            try
            {
                state = (KeysContainer)serializer.Deserialize(reader);
            }
            catch (InvalidCastException)
            {
                MessageBox.Show("Вы уверены, что открываете файл с состоянием машины?", "Ошибка", System.Windows.Forms.MessageBoxButtons.OK);
                return null;
            }
            catch (InvalidOperationException)
            {
                System.Windows.Forms.MessageBox.Show("Вы уверены, что открываете файл с состоянием машины?", "Ошибка", System.Windows.Forms.MessageBoxButtons.OK);
                return null;
            }
            finally
            {
                reader.Close();
            }
            return state;
        }

        public static KeysContainer SaveKeysContainer(string toFileName, ObservableCollection<Pair<int, String>> lst)
        {
            RecentFiles.KeysCollectionShared.AddFileToRecents(toFileName);


            KeysContainer state = new KeysContainer(lst);

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(state.GetType());

            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                serializer.Serialize(stream, state);
                stream.Position = 0;
                doc.Load(stream);
                doc.Save(toFileName);
            }
            return state;
        }

        public static (DeviceState, string) LoadDeviceState(string fileName)
        {
            RecentFiles.MachineStatesShared.AddFileToRecents(fileName);

            //CurrentFilePath = fileName;

            DeviceState state = new DeviceState();

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(state.GetType());

            System.IO.StreamReader reader = new System.IO.StreamReader(fileName);
            try
            {
                state = (DeviceState)serializer.Deserialize(reader);
            }
            catch (InvalidCastException)
            {
                reader.Close();
                MessageBox.Show("Вы уверены, что открываете файл с состоянием машины?", "Ошибка", System.Windows.Forms.MessageBoxButtons.OK);
                return (null, null);
            }
            catch (InvalidOperationException)
            {
                reader.Close();
                System.Windows.Forms.MessageBox.Show("Вы уверены, что открываете файл с состоянием машины?", "Ошибка", System.Windows.Forms.MessageBoxButtons.OK);
                return (null, null);
            }
            reader.Close();
            return (state, fileName);
        }

        public static DeviceState SaveDeviceState(string toFileName, List<KeyModel> lst)
        {
            RecentFiles.MachineStatesShared.AddFileToRecents(toFileName);


            DeviceState state = new DeviceState(lst);

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(state.GetType());

            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                serializer.Serialize(stream, state);
                stream.Position = 0;
                doc.Load(stream);
                doc.Save(toFileName);
            }
            return state;
        }
    }

    public static class Helper
    {
        public static ImageSource DarkPapSource = new BitmapImage(new Uri("pack://application:,,,/Images/image.jpg"));
        public static ImageSource PapSource = new BitmapImage(new Uri("pack://application:,,,/Images/texture_paper2.jpg"));
        public static ImageSource AluminumSource = new BitmapImage(new Uri("pack://application:,,,/Images/aluminum_texture1.jpg"));
        public static Regex reg = new Regex("[0-9]+");
        public static int MaxKeys = 200;

        public static List<T> Slice<T>(T[] source, int from, int len)
        {
            List<T> ans = new List<T>();
            for (int i = 0; i < len; ++i)
            {
                ans.Add(source[i + from]);
            }

            return ans;
        }

        public static List<T> CreateList<T>(int n, T def)
        {
            List<T> lst = new List<T>();
            for (int i = 0; i < n; ++i)
            {
                lst.Add(def);
            }

            return lst;
        }

        public static List<T> Concat<T>(List<T> f, List<T> s)
        {
            List<T> ans = new List<T>();
            for (int i = 0; i < f.Count; ++i)
            {
                ans.Add(f[i]);
            }

            for (int i = 0; i < s.Count; ++i)
            {
                ans.Add(s[i]);
            }

            return ans;
        }

        public static char ToUpper(char ch)
        {
            if (!(ch >= 'a' && ch <= 'z'))
            {
                return ch;
            }

            ch = (char)(ch - 'a' + 'A');
            return ch;
        }

        public static bool isUpperAlpha(char ch)
        {
            return ch >= 'A' && ch <= 'Z';
        }

        public static bool isAlphaString(string s)
        {
            for (int i = 0; i < s.Length; ++i)
            {
                if (!isAlpha(s[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static string Strip(string s)
        {
            if (s.Length == 0)
            {
                return s;
            }
            int st = 0, end = s.Length - 1;
            while (s[st] == ' ')
            {
                st++;
            }

            while (s[end] == ' ')
            {
                end--;
            }

            if (st > end)
            {
                return "";
            }
            else
            {
                return s.Substring(st, end - st + 1);
            }
        }

        public static bool isAlpha(char ch)
        {
            return Helper.isUpperAlpha(ch) || ch >= 'a' && ch <= 'z';
        }
    }

    public class IntToStringConverter : IValueConverter
    {
        string beginning;

        public IntToStringConverter()
        {
            beginning = "";
        }

        public IntToStringConverter(string str)
        {
            beginning = str;
        }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            // Do the conversion from bool to visibility
            return beginning + " " + ((int) value).ToString();
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            // Do the conversion from visibility to bool
            if (((String) value).Length == 0)
            {
                return Helper.MaxKeys;
            }
            return int.Parse((string)value);
        }
    }


    public class WarningIntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val = (int)value;
            if (val < Configuration.Message.Length)
            {
                return $"Недостаточно ключей, \n\rнужно минимум {Configuration.Message.Length}";
            } else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            // Do the conversion from visibility to bool
            var arr = ((string)value).Split();
            if (arr.Length == 0)
            {
                return 0;
            }
            var last = arr[arr.Length - 1];
            return int.Parse(last);
        }
    }


    public class LengthToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value > 0;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            // Do the conversion from visibility to bool
            return 1;
        }
    }

    public class WarningIntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val = (int)value;
            if (val < Configuration.Message.Length)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            // Do the conversion from visibility to bool
            return 1;
        }
    }

    public class IsEnabledToColorConverter : IValueConverter
    {
        Brush defaultColor;
        Brush disabledColor;
        public IsEnabledToColorConverter(Brush defaul, Brush disabled)
        {
            defaultColor = defaul;
            disabledColor = disabled;
        }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool val = (int)value >= Configuration.Message.Length;
            if (val)
            {
                return defaultColor;
            } else
            {
                return disabledColor;
            }
            //return defaultColor;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            // Do the conversion from visibility to bool
            return 1;
        }
    }

    public class LengthToColorConverter : IValueConverter
    {
        Brush defaultColor;
        Brush disabledColor;
        public LengthToColorConverter(Brush defaul, Brush disabled)
        {
            defaultColor = defaul;
            disabledColor = disabled;
        }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value > 0;
            //return defaultColor;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            // Do the conversion from visibility to bool
            return 1;
        }
    }

    public class CharToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            // Do the conversion from bool to visibility
            return ((Char)value).ToString();
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            // Do the conversion from visibility to bool
            if (((String)value).Length == 0)
            {
                return "";
            }
            return new string((char)value, 1);
        }
    }
}



static class AsyncUtils
{
    static public void DelayCall(int msec, Action fn)
    {
        // Grab the dispatcher from the current executing thread
        Dispatcher d = Dispatcher.CurrentDispatcher;

        // Tasks execute in a thread pool thread
        new Task(() => {
            System.Threading.Thread.Sleep(msec);   // delay

            // use the dispatcher to asynchronously invoke the action 
            // back on the original thread
            d.BeginInvoke(fn);
        }).Start();
    }
}



