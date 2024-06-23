using System;
using System.Collections.Generic;
using UnityEngine;

namespace Misc.ExtensionMethods
{
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if(source == null)
                Debug.LogException(new NullReferenceException());
            if(action == null)
                Debug.LogException(new NullReferenceException());

            foreach(var element in source)
            {
                action(element);
            }
        }

        public static int GetNullSafeHashCode<T>(this T value) where T : class
        {
            return value?.GetHashCode() ?? 1;
        }

        public static int GetConcatenateHash(params string[] list)
        {
            int hash = 17;
            foreach (string s in list)
            {
                hash = hash * 23 + s.GetHashCode();
            }
            return hash;
        }

        /// <summary>
        /// Concatenates multiple strings to see if a hash equals
        /// </summary>
        /// <returns>true if the same</returns>
        public static bool HashEqualsString(int hash, params string[] list)
        {
            int concatantedHash = GetConcatenateHash(list);
            return hash == concatantedHash;
        }
    }
}