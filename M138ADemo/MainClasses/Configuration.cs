using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace M138ADemo
{

    public class Pair<T, T1>
    {
        public T first;
        public T1 second;

        public Pair(T a, T1 b)
        {
            first = a;
            second = b;
        }

        public static Pair<T, T1> MakePair(T a, T1 b)
        {
            return new Pair<T, T1>(a, b);
        }
    }


    public static class Configuration
    {
        private static bool decrypt, encrypt, manual, automatic;

        public static SortedSet<int> forbiddenKeys = new SortedSet<int>();

        public static ObservableCollection<Pair<int, String>> lst = new ObservableCollection<Pair<int, string>>();

        public static int DecryptIndex { get; set; }

        public static int Count => lst.Count;


        public static bool Encrypt
        {
            get { return encrypt; }
            set
            {
                encrypt = value;
                decrypt = !value;
            }
        }

        public static bool Decrypt
        {
            get { return decrypt;}
            set { decrypt = value;
                encrypt = !value;
            }
        }

        public static bool Manual
        {
            get { return manual;}
            set
            {
                manual = value;
                automatic = !value;
            }
        }

        public static bool Automatic
        {
            get { return automatic;}
            set
            {
                automatic = value;
                manual = !value;
            }
        }

        public static string Message
        {
            get;
            set;
        }

        static Configuration()
        {
            lst.CollectionChanged += LstOnCollectionChanged;
        }

        private static void LstOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Pair<int, string> el in e.NewItems)
                {
                    forbiddenKeys.Add(el.first);
                }
            }

            if (e.OldItems != null)
            {

                foreach (Pair<int, string> el in e.OldItems)
                {
                    forbiddenKeys.Remove(el.first);
                }
            }

        }
    }
}
