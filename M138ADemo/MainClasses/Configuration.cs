using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace M138ADemo
{
    /// <summary>
    /// Configuration of current app state
    /// </summary>
    public static class Configuration
    {
        /// <summary>
        /// The decrypt flag.
        /// </summary>
        private static bool decrypt = false;
        /// <summary>
        /// The encrypt flag.
        /// </summary>
        private static bool encrypt = true;
        /// <summary>
        /// The manual flag.
        /// </summary>
        private static bool manual;
        /// <summary>
        /// The automatic flag.
        /// </summary>
        private static bool automatic;

        /// <summary>
        /// The forbidden keys.
        /// </summary>
        public static readonly SortedSet<int> forbiddenKeys = new SortedSet<int>();

        /// <summary>
        /// Gets or sets the current key list.
        /// </summary>
        /// <value>The key list.</value>
        public static ObservableCollection<(int, String)> KeyList { get; set; } = new ObservableCollection<(int, string)>();

        /// <summary>
        /// Gets or sets the state of the device.
        /// </summary>
        /// <value>The state of the device.</value>
        public static DeviceState DeviceState { get; set; } = null;

        /// <summary>
        /// Gets or sets the decrypt index.
        /// </summary>
        /// <value>The index of the decrypt.</value>
        public static int DecryptIndex { get; set; }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public static int Count => KeyList.Count;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:M138ADemo.Configuration"/> is compact work space.
        /// </summary>
        /// <value><c>true</c> if is compact work space; otherwise, <c>false</c>.</value>
        public static bool IsCompactWorkSpace { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:M138ADemo.Configuration"/> is encrypt.
        /// </summary>
        /// <value><c>true</c> if encrypt; otherwise, <c>false</c>.</value>
        public static bool Encrypt
        {
            get { return encrypt; }
            set
            {
                encrypt = value;
                decrypt = !value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:M138ADemo.Configuration"/> is decrypt.
        /// </summary>
        /// <value><c>true</c> if decrypt; otherwise, <c>false</c>.</value>
        public static bool Decrypt
        {
            get { return decrypt;}
            set { decrypt = value;
                encrypt = !value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:M138ADemo.Configuration"/> is manual.
        /// </summary>
        /// <value><c>true</c> if manual; otherwise, <c>false</c>.</value>
        public static bool Manual
        {
            get { return manual;}
            set
            {
                manual = value;
                automatic = !value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:M138ADemo.Configuration"/> is automatic.
        /// </summary>
        /// <value><c>true</c> if automatic; otherwise, <c>false</c>.</value>
        public static bool Automatic
        {
            get { return automatic;}
            set
            {
                automatic = value;
                manual = !value;
            }
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public static string Message
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes the <see cref="T:M138ADemo.Configuration"/> class.
        /// </summary>
        static Configuration()
        {
            KeyList.CollectionChanged += LstOnCollectionChanged;
        }

        /// <summary>
        /// Update forbidden Keys the on collection changed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private static void LstOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach ((int, string) el in e.NewItems)
                {
                    forbiddenKeys.Add(el.Item1);
                }
            }

            if (e.OldItems != null)
            {

                foreach ((int, string) el in e.OldItems)
                {
                    forbiddenKeys.Remove(el.Item1);
                }
            }

        }
    }
}
