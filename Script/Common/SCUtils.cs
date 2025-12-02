using System;
using System.Collections.Generic;
using System.Text;
using Data;

namespace Script
{
    public class SCUtils
    {
        /**
         * 返回将value限定于[min,max]区间
         */
        public int clamp(int value, int min, int max)
        {
            if (!(value > min))
            {
                return min;
            }
            if (value < max)
            {
                return value;
            }
            return max;
        }

        public static int WeightedRandomSimple(Random random, int length, Func<int, int> getWeight)
        {
            int totalWeight = 0;
            for (int i = 0; i < length; i++)
            {
                totalWeight += getWeight(i);
            }
            if (totalWeight == 0)
            {
                return -1;
            }

            return WeightedRandom(random, totalWeight, length, getWeight);
        }

        public static int WeightedRandom(Random random, int totalWeight, int length, Func<int, int> getWeight)
        {
            int randomValue = Random(random, min: 0, max: totalWeight);
            int currentTotal = 0;

            for (int i = 0; i < length; i++)
            {
                currentTotal += getWeight(i);
                if (randomValue < currentTotal)
                {
                    return i;
                }
            }
            return 0;
        }

        // 如果 totalWeight 可能超过 int 就用这个版本
        public static int WeightedRandom_long(Random random, long totalWeight, int length, Func<int, long> getWeight)
        {
            long randomValue = (long)(totalWeight * random.NextDouble());
            long currentTotal = 0;

            for (int i = 0; i < length; i++)
            {
                currentTotal += getWeight(i);
                if (randomValue < currentTotal)
                {
                    return i;
                }
            }
            return 0;
        }

        // wantedCount > length
        public static void RandomMany(Random random, int length, int wantedCount, Action<int> onSelect)
        {
            var indices = new int[length];
            for (int i = 0; i < length; i++)
            {
                indices[i] = i;
            }

            int L = indices.Length;
            for (int i = 0; i < wantedCount; i++)
            {
                int j = random.Next(0, L);
                onSelect(indices[j]);

                indices[j] = indices[L - 1];
                L--;
            }
        }

        // 分段随机，分成 segments 段，每段取 1 个
        // 如果 wantedCount < segments，某些段就没有
        // 如果 wantedCount > segments，某些段有多个
        // 结果不重复
        public static void RandomMany_Segments(Random random, int length, int wantedCount, int segments, Action<int> onSelect)
        {
            if (length <= segments)
            {
                RandomMany(random, length, wantedCount, onSelect);
                return;
            }

            var lists = new List<List<int>>();
            int perSeg = length / segments; // 每段几个
            MyDebug.Assert(perSeg > 0);

            for (int i = 0; i < segments; i++)
            {
                int start = i * perSeg;
                var list = new List<int>();
                if (i < segments - 1)
                {
                    for (int j = start; j < start + perSeg; j++)
                    {
                        list.Add(j);
                    }
                }
                else
                {
                    for (int j = start; j < length; j++)
                    {
                        list.Add(j);
                    }
                }
                lists.Add(list);
            }

            int s_index = 0;
            bool phase2 = false;
            while (wantedCount > 0)
            {
                if (!phase2)
                {
                    List<int> list = lists[s_index];
                    int hitIndex = random.Next(0, list.Count);
                    onSelect(list[hitIndex]);
                    wantedCount--;
                    list.RemoveAt(hitIndex);

                    s_index++;
                    if (s_index >= lists.Count)
                    {
                        phase2 = true;

                        for (int i = 0; i < lists.Count; i++)
                        {
                            if (lists[i].Count == 0)
                            {
                                lists.RemoveAt(i);
                                i--;
                            }
                        }

                        if (lists.Count == 0)
                        {
                            // end
                            break;
                        }
                    }
                }
                else
                {
                    s_index = random.Next(0, lists.Count);
                    var list = lists[s_index];
                    int hitIndex = random.Next(0, list.Count);
                    onSelect(list[hitIndex]);
                    wantedCount--;
                    list.RemoveAt(hitIndex);

                    if (list.Count == 0)
                    {
                        lists.RemoveAt(s_index);

                        if (lists.Count == 0)
                        {
                            // end
                            break;
                        }
                    }

                }
            }
        }

        public static bool sRandomHit(Random random, int total, int n)
        {
            return random.Next(total) < n;
        }
        // 随机，概率是 n/total，命中返回 true
        public bool RandomHit(Random random, int total, int n)
        {
            return this.Random(random, total) < n;
        }

        // [min, max)
        public static int RandomSquared(Random random, int min, int max)
        {
            double n = random.NextDouble(); // [0, 1)
            n = n * n;
            double r = min + (max - min) * n;
            return (int)r;
        }

        // [min, max)
        public static int Random(Random random, int min, int max)
        {
            return random.Next(min, max);
        }

        // [0, max)
        public int Random(Random random, int max)
        {
            return random.Next(max);
        }

        // [min, max)
        public float RandomFloat(Random random, float min, float max)
        {
            return (float)(random.NextDouble() * (max - min) + min);
        }

        // [0f, 1f)
        public float RandomFloat01(Random random)
        {
            return (float)random.NextDouble();
        }


        static double GenerateNormalRandom(Random rand, double mean, double stdDev)
        {
            // Box-Muller transform
            double u1 = 1.0 - rand.NextDouble(); // [0,1)
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); // random normal(0,1)
            double randNormal = mean + stdDev * randStdNormal; // random normal(mean,stdDev^2)

            return randNormal;
        }

        // [min, max]
        public static int GenerateNormalRandomInRange(Random rand, int min, int max)
        {
            return (int)GenerateNormalRandomInRange(rand, (double)min, 0.999 + (double)max);
        }

        // [min, max)
        public static double GenerateNormalRandomInRange(Random rand, double min, double max)
        {
            double mean = (min + max) / 2.0;
            double stdDev = (max - min) / 6.0; // 99.7% of values will lie within ±3 standard deviations

            double randNormal;
            int c = 0;
            do
            {
                randNormal = GenerateNormalRandom(rand, mean, stdDev);
            }
            while ((randNormal < min || randNormal > max) && c++ <= 5);

            // Clamp the result to the [min, max] range
            return Math.Max(min, Math.Min(max, randNormal));
        }

        // hello_{project} ====> hello_pkcastles
        public static string ReplaceVariable(string F, Func<string, string> getValue)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < F.Length; i++)
            {
                char c = F[i];
                if (c == '{')
                {
                    int j = F.IndexOf('}', i + 1);
                    if (j < 0)
                    {
                        throw new Exception("ReplaceVariable j < 0");
                    }
                    string key = F.Substring(i + 1, j - i - 1);
                    string value = getValue(key);
                    if (value == null)
                    {
                        throw new Exception("ReplaceVariable unknown key: " + key);
                    }
                    sb.Append(value);

                    i = j;
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static bool HasDuplicateElement<T>(List<T> list) where T : IEquatable<T>
        {
            for (int i = 1; i < list.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (list[i].Equals(list[j]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // https://stackoverflow.com/questions/11743160/how-do-i-encode-and-decode-a-base64-string
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}