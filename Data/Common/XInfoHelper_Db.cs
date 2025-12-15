using System.Numerics;
using System.Runtime.CompilerServices;

public static class XInfoHelper_Db
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int? Copy_int(int other)
    {
        return other == 0 ? (int?)null : other;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long? Copy_long(long other)
    {
        return other == 0 ? (long?)null : other;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool? Copy_bool(bool other)
    {
        return other == false ? (bool?)null : other;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float? Copy_float(float other)
    {
        return other == 0f ? (float?)null : other;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BigInteger? Copy_BigInteger(BigInteger other)
    {
        return other == 0 ? (BigInteger?)null : other;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Copy_string(string other)
    {
        return string.IsNullOrEmpty(other) ? null : other;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Copy_Enum<T>(T other) where T : Enum
    {
        return other;
    }

    public static T_Db Copy_Class<T_Db, T>(T other)
        where T_Db : class, IIsDifferent_Db<T>, new()
        where T : class
    {
        if (other == null)
        {
            return null;
        }
        T_Db self = new T_Db();
        if (!self.DeepCopyFrom(other))
        {
            return null;
        }
        
        return self;
    }

    public static List<T> Copy_ListValue<T>(List<T> other)
    {
        if (other == null || other.Count == 0)
        {
            return null;
        }

        var self = new List<T>();
        XInfoHelper.DeepCopyFrom_ListValue(self, other);
        return self;
    }

    public static List<T_Db> Copy_ListClass<T_Db, T>(List<T> other)
        where T_Db : class, IIsDifferent_Db<T>, new()
        where T : class, IIsDifferent<T>, new()
    {
        if (other == null || other.Count == 0)
        {
            return null;
        }

        var self = new List<T_Db>();
        DeepCopyFrom_ListClass(self, other);
        return self;
    }

    public static HashSet<T> Copy_HashSetValue<T>(HashSet<T> other)
    {
        if (other == null || other.Count == 0)
        {
            return null;
        }

        var self = new HashSet<T>();
        XInfoHelper.DeepCopyFrom_HashSetValue(self, other);
        return self;
    }

    public static Dictionary<K, V> Copy_DictValue<K, V>(Dictionary<K, V> other)
    {
        if (other == null || other.Count == 0)
        {
            return null;
        }

        var self = new Dictionary<K, V>();
        XInfoHelper.DeepCopyFrom_DictValue(self, other);
        return self;
    }

    static void DeepCopyFrom_ListClass<T_Db, T>(this List<T_Db> self, List<T> other)
        where T_Db : class, IIsDifferent_Db<T>, new()
        where T : class, IIsDifferent<T>, new()
    {
        while (self.Count > other.Count)
        {
            self.RemoveAt(self.Count - 1);
        }
        for (int i = 0; i < other.Count; i++)
        {
            if (self.Count <= i)
            {
                T_Db t = new T_Db();
                self.Add(t);
            }
            self[i].DeepCopyFrom(other[i]);
        }
    }

    public static Dictionary<K, V_Db> Copy_DictClass<K, V_Db, V>(Dictionary<K, V> other)
        where V_Db : class, IIsDifferent_Db<V>, new()
        where V : class, IIsDifferent<V>, new()
    {
        if (other == null || other.Count == 0)
        {
            return null;
        }

        var self = new Dictionary<K, V_Db>();
        DeepCopyFrom_DictClass(self, other);
        return self;
    }

    static void DeepCopyFrom_DictClass<K, V_Db, V>(this Dictionary<K, V_Db> self, Dictionary<K, V> other)
        where V_Db : class, IIsDifferent_Db<V>, new()
        where V : class, IIsDifferent<V>, new()
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
                V_Db tvalue = new V_Db();
                self[key] = tvalue;
            }
            self[key].DeepCopyFrom(other[key]);
        }
    }
}