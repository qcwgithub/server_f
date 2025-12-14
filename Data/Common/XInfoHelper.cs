using Data;
using System;
using System.Collections.Generic;

public static class XInfoHelper
{
    public static void DeepCopyFrom_ListValue<T>(this List<T> self, List<T> other)
    {
        while (self.Count > other.Count)
        {
            self.RemoveAt(self.Count - 1);
        }
        for (int i = 0; i < other.Count; i++)
        {
            if (self.Count <= i)
            {
                self.Add(default(T));
            }
            self[i] = other[i];
        }
    }
    public static void DeepCopyFrom_HashSetValue<T>(this HashSet<T> self, HashSet<T> other)
    {
        self.Clear();
        foreach (var v in other)
        {
            self.Add(v);
        }
    }

    public static void DeepCopyFrom_ListClass<T>(this List<T> self, List<T> other) where T : class, IIsDifferent<T>, new()
    {
        while (self.Count > other.Count)
        {
            self.RemoveAt(self.Count - 1);
        }
        for (int i = 0; i < other.Count; i++)
        {
            if (self.Count <= i)
            {
                T t = new T();
                t.Ensure();
                self.Add(t);
            }
            self[i].DeepCopyFrom(other[i]);
        }
    }
    public static bool IsDifferent_ListValue<T>(this List<T> self, List<T> other) where T : IEquatable<T>
    {
        if (self.Count != other.Count)
        {
            return true;
        }
        for (int i = 0; i < self.Count; i++)
        {
            if (!self[i].Equals(other[i]))
            {
                return true;
            }
        }
        return false;
    }
    public static bool IsDifferent_HashSetValue<T>(this HashSet<T> self, HashSet<T> other) where T : IEquatable<T>
    {
        if (self.Count != other.Count)
        {
            return true;
        }
        foreach (var v in self)
        {
            if (!other.Contains(v))
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsDifferent_ListClass<T>(this List<T> self, List<T> other
#if UNITY_2017_1_OR_NEWER
        , List<string> tracker = null
#endif
) where T : class, IIsDifferent<T>
    {
        if (self.Count != other.Count)
        {
#if UNITY_2017_1_OR_NEWER
            if (tracker != null) tracker.Add($"self.Count({self.Count}) != other.Count({other.Count})");
#endif
            return true;
        }
        for (int i = 0; i < self.Count; i++)
        {
            if (self[i].IsDifferent(other[i]
#if UNITY_2017_1_OR_NEWER
                , tracker
#endif
            ))
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsDifferent_DictValue<K, V>(this Dictionary<K, V> self, Dictionary<K, V> other) where V : IEquatable<V>
    {
        if (self.Count != other.Count)
        {
            return true;
        }
        foreach (var kv in self)
        {
            if (!other.ContainsKey(kv.Key))
            {
                return true;
            }
            if (kv.Value == null)
            {
                if (other[kv.Key] != null)
                {
                    return true;
                }
            }
            else if (!kv.Value.Equals(other[kv.Key]))
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsDifferent_DictEnum<K, V>(this Dictionary<K, V> self, Dictionary<K, V> other) where V : System.Enum
    {
        if (self.Count != other.Count)
        {
            return true;
        }
        foreach (var kv in self)
        {
            if (!other.ContainsKey(kv.Key))
            {
                return true;
            }
            if (!kv.Value.Equals(other[kv.Key]))
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsDifferent_DictClass<K, V>(this Dictionary<K, V> self, Dictionary<K, V> other
#if UNITY_2017_1_OR_NEWER
        , List<string> tracker = null
#endif
    ) where V : class, IIsDifferent<V>
    {
        if (self.Count != other.Count)
        {
#if UNITY_2017_1_OR_NEWER
            if (tracker != null) tracker.Add($"self.Count({self.Count}) != other.Count({other.Count})");
#endif
            return true;
        }
        foreach (var kv in self)
        {
            if (!other.ContainsKey(kv.Key))
            {
#if UNITY_2017_1_OR_NEWER
                if (tracker != null) tracker.Add($"!other.ContainsKey(kv.Key({kv.Key}))");
#endif
                return true;
            }
            if (kv.Value.IsDifferent(other[kv.Key]
#if UNITY_2017_1_OR_NEWER
                , tracker
#endif
            ))
            {
                return true;
            }
        }
        return false;
    }
    public static void DeepCopyFrom_DictValue<K, V>(this Dictionary<K, V> self, Dictionary<K, V> other)
    {
        {
            List<K> temp = null;
            foreach (var kv in self)
            {
                if (!other.ContainsKey(kv.Key))
                {
                    if (temp == null) temp = new List<K>();
                    temp.Add(kv.Key);
                }
            }
            if (temp != null)
            {
                foreach (var k in temp)
                {
                    self.Remove(k);
                }
            }
        }
        foreach (var kv in other)
        {
            var key = kv.Key;
            if (!self.ContainsKey(key))
            {
                self[key] = default(V);
            }
            self[key] = other[key];
        }
    }

    public static void DeepCopyFrom_DictList<K, V>(this Dictionary<K, List<V>> self, Dictionary<K, List<V>> other) where V : struct
    {
        self.Clear();
        foreach (var pair in other)
        {
            self[pair.Key] = new List<V>();
            self[pair.Key].AddRange(pair.Value);
        }
    }

    public static void DeepCopyFrom_DictClass<K, V>(this Dictionary<K, V> self, Dictionary<K, V> other) where V : class, IIsDifferent<V>, new()
    {
        {
            List<K> temp = null;
            foreach (var kv in self)
            {
                if (!other.ContainsKey(kv.Key))
                {
                    if (temp == null) temp = new List<K>();
                    temp.Add(kv.Key);
                }
            }
            if (temp != null)
            {
                foreach (var k in temp)
                {
                    self.Remove(k);
                }
            }
        }
        foreach (var kv in other)
        {
            var key = kv.Key;
            if (!self.ContainsKey(key))
            {
                V tvalue = new V();
                tvalue.Ensure();
                self[key] = tvalue;
            }
            self[key].DeepCopyFrom(other[key]);
        }
    }
}