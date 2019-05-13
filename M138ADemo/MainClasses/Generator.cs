using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M138ADemo
{

    /// <summary>
    /// Class-extension for shuffling array
    /// </summary>
    public static class RandomShuffler
    {
        public static void Shuffle<T>(this IList<T> list, Random rnd)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }

    /// <summary>
    /// Class for generating random Keys
    /// </summary>
    public static class Generator
    {
        ///<summary>
        /// Max number of keys allowed to generate
        ///</summary>
        private readonly static int maxNumberOfKeys = 100;

        ///<summary>
        /// Generates random keys
        ///</summary>
        ///<param name="numberOfKeys">amount of keys to generate</param>
        ///<param name="seed"> seed for random </param>
        ///<returns> generated keys </returns>
        public static ObservableCollection<(int, string)> GenerateRandomKeys(int numberOfKeys = 20, int seed = -1)
        {
            Random rnd;
            if (seed != -1)
            {
                rnd = new Random(seed);
            }
            else
            {
                rnd = new Random();
            }
            numberOfKeys = Math.Min(maxNumberOfKeys, numberOfKeys);
            var list = new ObservableCollection<(int, string)>();
            var set = new SortedSet<int>();
            for (int i = 0; i < numberOfKeys; ++i)
            {
                var charSet = new List<char>();
                for (int ch = 0; ch < 26; ++ch)
                {
                    charSet.Add((char)(ch + 'a'));
                }
                charSet.Shuffle(rnd);
                string curr = new string(charSet.ToArray());
                

                int num = 0;
                while (set.Contains(num = rnd.Next(0, 200))) { }

                set.Add(num);
                list.Add((num, curr));
            }

            return list;
        }

    }
}
