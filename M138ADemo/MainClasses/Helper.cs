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
using System.IO;
using M138ADemo.MainClasses;

namespace M138ADemo
{
    /// <summary>
    /// IO Helper.
    /// </summary>
    public static class IOHelper
    {
        /// <summary>
        /// Loads the keys container.
        /// </summary>
        /// <returns>The keys container.</returns>
        /// <param name="fileName">File name.</param>
        public static KeysContainer LoadKeysContainer(string fileName)
        {

            //CurrentFilePath = fileName;

            KeysContainer state = new KeysContainer();

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(state.GetType());

            var fillInfo = new FileInfo(fileName);
            if (!fillInfo.Exists)
            {
                fillInfo.Create();
            }

            using (System.IO.StreamReader reader = new System.IO.StreamReader(fileName)) {
                try
                {
                    state = (KeysContainer)serializer.Deserialize(reader);
                    RecentFiles.KeysCollectionShared.AddFileToRecents(fileName);
                }
                catch (InvalidCastException)
                {
                    MessageBox.Show("Вы уверены, что открываете файл с ключами?", "Ошибка", System.Windows.Forms.MessageBoxButtons.OK);
                    RecentFiles.KeysCollectionShared.DeleteFileFromRecents(fileName);
                    return null;
                }
                catch (InvalidOperationException)
                {
                    System.Windows.Forms.MessageBox.Show("Вы уверены, что открываете файл с ключами?", "Ошибка", System.Windows.Forms.MessageBoxButtons.OK);
                    RecentFiles.KeysCollectionShared.DeleteFileFromRecents(fileName);
                    return null;
                }
            }
            return state;
        }

        /// <summary>
        /// Saves the keys container.
        /// </summary>
        /// <returns>The keys container.</returns>
        /// <param name="toFileName">To file name.</param>
        /// <param name="lst">Lst.</param>
        public static KeysContainer SaveKeysContainer(string toFileName, ObservableCollection<(int, String)> lst)
        {
            RecentFiles.KeysCollectionShared.AddFileToRecents(toFileName);


            KeysContainer state = new KeysContainer(lst);

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(state.GetType());


            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                try
                {
                    serializer.Serialize(stream, state);
                    stream.Position = 0;
                    doc.Load(stream);
                    doc.Save(toFileName);
                }
                catch (System.Xml.XmlException ex)
                {
                    MessageBox.Show("Произошла ошибка при сохранении в файл");
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show("Нет доступа к файлу");
                }
            }
            return state;
        }

        /// <summary>
        /// Loads the state of the device.
        /// </summary>
        /// <returns>The device state.</returns>
        /// <param name="fileName">File name.</param>
        public static (DeviceState, string) LoadDeviceState(string fileName)
        {

            //CurrentFilePath = fileName;

            DeviceState state = new DeviceState();

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(state.GetType());
            var fillInfo = new FileInfo(fileName);
            if (!fillInfo.Exists)
            {
                fillInfo.Create();
            }

            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(fileName))
                {
                    try
                    {
                        state = (DeviceState)serializer.Deserialize(reader);
                        RecentFiles.MachineStatesShared.AddFileToRecents(fileName);
                    }
                    catch (InvalidCastException)
                    {
                        MessageBox.Show("Вы уверены, что открываете файл с состоянием машины?", "Ошибка", System.Windows.Forms.MessageBoxButtons.OK);
                        RecentFiles.MachineStatesShared.DeleteFileFromRecents(fileName);
                        return (null, null);
                    }
                    catch (InvalidOperationException)
                    {
                        System.Windows.Forms.MessageBox.Show("Вы уверены, что открываете файл с состоянием машины?", "Ошибка", System.Windows.Forms.MessageBoxButtons.OK);
                        RecentFiles.MachineStatesShared.DeleteFileFromRecents(fileName);
                        return (null, null);
                    }
                }
            }
            catch (System.IO.IOException ex)
            {
                System.Windows.Forms.MessageBox.Show("Кажется, что файл удален?", "Ошибка", System.Windows.Forms.MessageBoxButtons.OK);
                RecentFiles.MachineStatesShared.DeleteFileFromRecents(fileName);
                return (null, null);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show("Неправильное имя файла", "Ошибка", System.Windows.Forms.MessageBoxButtons.OK);
                RecentFiles.MachineStatesShared.DeleteFileFromRecents(fileName);
                return (null, null);
            }
            return (state, fileName);
        }

        /// <summary>
        /// Saves the state of the device.
        /// </summary>
        /// <returns>The device state.</returns>
        /// <param name="toFileName">file name where to save.</param>
        /// <param name="lst">List of models</param>
        public static DeviceState SaveDeviceState(string toFileName, List<KeyModel> lst)
        {
            RecentFiles.MachineStatesShared.AddFileToRecents(toFileName);


            DeviceState state = new DeviceState(lst);

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(state.GetType());
            

            try
            {
                using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                {
                    serializer.Serialize(stream, state);
                    stream.Position = 0;
                    doc.Load(stream);
                    doc.Save(toFileName);
                }
                return state;
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("Не получается сохранить файл", "Ошибка", MessageBoxButtons.OK);
                Console.WriteLine(ex.Message);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show("Кажется, вы пытаетесь сохранить пустое состояние машины", "Ошибка", MessageBoxButtons.OK);
                Console.WriteLine(ex.Message);
            }
            catch (System.Xml.XmlException ex)
            {
                MessageBox.Show("НЕ получается записать состояние в файл", "Ошибка", MessageBoxButtons.OK);
                Console.WriteLine(ex.Message);
            }
            return null;
        }
    }

    /// <summary>
    /// Helper.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// The dark paper image source.
        /// </summary>
        public static ImageSource DarkPapSource = new BitmapImage(new Uri("pack://application:,,,/Images/image.jpg"));

        /// <summary>
        /// The paper images source.
        /// </summary>
        public static ImageSource PapSource = new BitmapImage(new Uri("pack://application:,,,/Images/texture_paper2.jpg"));

        /// <summary>
        /// The aluminum source.
        /// </summary>
        public static ImageSource AluminumSource = new BitmapImage(new Uri("pack://application:,,,/Images/aluminum_texture1.jpg"));

        /// <summary>
        /// The border images source.
        /// </summary>
        public static ImageSource BorderSource = new BitmapImage(new Uri("pack://application:,,,/Images/borderimage.jpg"));

        /// <summary>
        /// The regex for checking that string containts only digits
        /// </summary>
        public static Regex reg = new Regex("[0-9]+");

        /// <summary>
        /// The max number of keys.
        /// </summary>
        public static int MaxKeys = 100;

        /// <summary>
        /// Creates the list.
        /// </summary>
        /// <returns>The list.</returns>
        /// <param name="n">N.</param>
        /// <param name="def">Def.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static List<T> CreateList<T>(int n, T def)
        {
            List<T> lst = new List<T>();
            for (int i = 0; i < n; ++i)
            {
                lst.Add(def);
            }

            return lst;
        }

        /// <summary>
        /// Makes char in upperCase.
        /// </summary>
        /// <returns>The upperCase char.</returns>
        /// <param name="ch">Ch.</param>
        public static char ToUpper(char ch)
        {
            if (!(ch >= 'a' && ch <= 'z'))
            {
                return ch;
            }

            ch = (char)(ch - 'a' + 'A');
            return ch;
        }

        /// <summary>
        /// Is the alpha in uppercase.
        /// </summary>
        /// <returns><c>true</c>, if alpha was upperCase, <c>false</c> otherwise.</returns>
        /// <param name="ch">Ch.</param>
        public static bool isUpperAlpha(char ch)
        {
            return ch >= 'A' && ch <= 'Z';
        }

        /// <summary>
        /// Checks that the string containts only alphabetic symblos.
        /// </summary>
        /// <returns><c>true</c>, if string was constructed from alphabetic symbols, <c>false</c> otherwise.</returns>
        /// <param name="s">S.</param>
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

        /// <summary>
        /// Is the alphabetic symbol.
        /// </summary>
        /// <returns><c>true</c>, if char was alphabetic, <c>false</c> otherwise.</returns>
        /// <param name="ch">Ch.</param>
        public static bool isAlpha(char ch)
        {
            return (ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z');
        }
    }

    /// <summary>
    /// Inverse bool converter.
    /// </summary>
    public class InverseBoolConverter: IValueConverter
    {
        /// <summary>
        /// Convert the specified value, targetType, parameter and culture.
        /// </summary>
        /// <returns>The convert.</returns>
        /// <param name="value">Value.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="culture">Culture.</param>
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <returns>The back.</returns>
        /// <param name="value">Value.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="culture">Culture.</param>
        public object ConvertBack(object value, Type targetType,
           object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

    }

    /// <summary>
    /// Int to string converter.
    /// </summary>
    public class IntToStringConverter : IValueConverter
    {
        /// <summary>
        /// The prefix.
        /// </summary>
        string beginning;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.IntToStringConverter"/> class.
        /// </summary>
        public IntToStringConverter()
        {
            beginning = "";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.IntToStringConverter"/> class.
        /// </summary>
        /// <param name="str">Needed prefix.</param>
        public IntToStringConverter(string str)
        {
            beginning = str;
        }

        /// <summary>
        /// Convert the specified value, targetType, parameter and culture.
        /// </summary>
        /// <returns>The convert.</returns>
        /// <param name="value">Value.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="culture">Culture.</param>
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return beginning + ((int) value).ToString();
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <returns>The back.</returns>
        /// <param name="value">Value.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="culture">Culture.</param>
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (((string) value).Length == 0)
            {
                return null;
            }
            return int.Parse((string)value);
        }
    }

    /// <summary>
    /// Warning int to string converter.
    /// </summary>
    public class WarningIntToStringConverter : IValueConverter
    {
        /// <summary>
        /// Convert the specified value, targetType, parameter and culture.
        /// </summary>
        /// <returns>The convert.</returns>
        /// <param name="value">Value.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="culture">Culture.</param>
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

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <returns>The back.</returns>
        /// <param name="value">Value.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="culture">Culture.</param>
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

    /// <summary>
    /// Warning int to bool converter.
    /// </summary>
    public class WarningIntToBoolConverter : IValueConverter
    {
        /// <summary>
        /// Convert the specified value, targetType, parameter and culture.
        /// </summary>
        /// <returns>The convert.</returns>
        /// <param name="value">Value.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="culture">Culture.</param>
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

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <returns>The back.</returns>
        /// <param name="value">Value.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="culture">Culture.</param>
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            // Do the conversion from visibility to bool
            return 1;
        }
    }

    /// <summary>
    /// Is enabled to color converter.
    /// </summary>
    public class IsEnabledToColorConverter : IValueConverter
    {
        /// <summary>
        /// The default color.
        /// </summary>
        Brush defaultColor;

        /// <summary>
        /// The color of the disabled.
        /// </summary>
        Brush disabledColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.IsEnabledToColorConverter"/> class.
        /// </summary>
        /// <param name="defaul">Defaul.</param>
        /// <param name="disabled">Disabled.</param>
        public IsEnabledToColorConverter(Brush defaul, Brush disabled)
        {
            defaultColor = defaul;
            disabledColor = disabled;
        }

        /// <summary>
        /// Convert the specified value, targetType, parameter and culture.
        /// </summary>
        /// <returns>The convert.</returns>
        /// <param name="value">Value.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="culture">Culture.</param>
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

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <returns>The back.</returns>
        /// <param name="value">Value.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="culture">Culture.</param>
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            // Do the conversion from visibility to bool
            return 1;
        }
    }

    /// <summary>
    /// Length to color converter.
    /// </summary>
    public class LengthToColorConverter : IValueConverter
    {
        /// <summary>
        /// The default color.
        /// </summary>
        Brush defaultColor;
        Brush disabledColor;
        public LengthToColorConverter(Brush defaul, Brush disabled)
        {
            defaultColor = defaul;
            disabledColor = disabled;
        }

        /// <summary>
        /// Convert the specified value, targetType, parameter and culture.
        /// </summary>
        /// <returns>The convert.</returns>
        /// <param name="value">Value.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="culture">Culture.</param>
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


/// <summary>
/// Async utils.
/// </summary>
static class AsyncUtils
{
    /// <summary>
    /// Delays the call.
    /// </summary>
    /// <param name="msec">Msec.</param>
    /// <param name="fn">Fn.</param>
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



