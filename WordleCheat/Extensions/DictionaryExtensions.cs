using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleCheat.Extensions
{
    public static class DictionaryExtensions
    { 
        public static void AddOrSumValue<TKey>(this Dictionary<TKey, int> dictionary, TKey key, int value)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] += value;
            else
                dictionary[key] = value;
        }

        public static void AddOrSumValue<TKey>(this ConcurrentDictionary<TKey, int> dictionary, TKey key, int value)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] += value;
            else
                dictionary[key] = value;
        }
    }
}
