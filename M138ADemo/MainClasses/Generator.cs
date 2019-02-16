using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M138ADemo
{
    public static class Generator
    {
        private readonly static int maxNumberOfKeys = 100;
        public static ObservableCollection<Pair<int, string>> GenerateRandomKeys(int numberOfKeys = 20, int seed = -1)
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
            var list = new ObservableCollection<Pair<int, string>>();
            var set = new SortedSet<int>(Configuration.forbiddenKeys);
            for (int i = 0; i < numberOfKeys; ++i)
            {
                var charSet = new SortedSet<char>();
                string curr = "";
                for (int j = 0; j < 26; ++j)
                {
                    char ch;
                    while (charSet.Contains(ch = (char)(rnd.Next(0, 26) + 'a')))
                    {
                    }

                    curr += ch;
                    charSet.Add(ch);
                }

                int num = 0;
                while (set.Contains(num = rnd.Next(0, 200))) { }

                set.Add(num);
                list.Add(Pair<int, string>.MakePair(num, curr));
            }

            return list;
        }

    }
}
