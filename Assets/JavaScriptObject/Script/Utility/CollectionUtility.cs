using System.Collections.Generic;

namespace CrossPlatformJson
{
    public class CollectionUtility
    {
        public static bool CompareX<TKey, TValue>(Dictionary<TKey, TValue> dict1, Dictionary<TKey, TValue> dict2)
        {
            if (dict1 == dict2) return true;
            if (dict1 == null || dict2 == null) return false;
            if (dict1.Count != dict2.Count) return false;

            var valueComparer = EqualityComparer<TValue>.Default;

            foreach (var kvp in dict1)
            {
                TValue value2;
                if (!dict2.TryGetValue(kvp.Key, out value2)) return false;
                if (!valueComparer.Equals(kvp.Value, value2)) return false;
            }

            return true;
        }

        public static bool CompareX<TValue>(List<TValue> list1, List<TValue> list2)
        {
            if (list1 == list2) return true;
            if (list1 == null || list2 == null) return false;
            if (list1.Count != list2.Count) return false;

            var valueComparer = EqualityComparer<TValue>.Default;

            for (var i = 0; i < list1.Count; i++)
            {
                var value1 = list1[i];
                var value2 = list1[i];
                if (!valueComparer.Equals(value1, value2)) return false;
            }

            return true;
        }
    }
}