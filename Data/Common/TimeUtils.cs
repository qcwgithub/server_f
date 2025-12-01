using System.IO;
using System;
using System.Diagnostics;
using System.Numerics;

namespace Data
{
    public struct AbsoluteTimeOptions
    {
        public bool ignoreSeconds;
    }

    public static class TimeUtils
    {
        public const int ONE_DAYS = 1 * 86400;
        public const int THREE_DAYS = 3 * 86400;
        public const int SEVEN_DAYS = 7 * 86400;
        public const int THIRTY_DAYS = 30 * 86400;

        // 取时间戳时，基于此时间
        public static readonly DateTime s_baseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        static void AssertIsUtc(DateTime dt)
        {
            if (dt.Kind != DateTimeKind.Utc)
            {
#if UNITY_2017_1_OR_NEWER
                UnityEngine.Debug.LogError("dt.Kind != Utc");
#else
                ConsoleColor old = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("dt.Kind != Utc");
                Console.ForegroundColor = old;
#endif
            }
        }

        public static DateTime SecondsToDateTime(int s)
        {
            DateTime dt = s_baseDate.AddSeconds(s);
            AssertIsUtc(dt);
            return dt;
        }

        public static int DateTimeToSeconds(DateTime dt)
        {
            AssertIsUtc(dt);
            return (int)(dt - s_baseDate).TotalSeconds;
        }

        public static DateTime CreateDateTime(int year, int month, int day)
        {
            return new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
        }

        public static string ToString(int timeS)
        {
            DateTime dt = SecondsToDateTime(timeS);
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        const long TF = 10000000000;

        public static double AddTimeFactor(double oriNum)
        {
            return (double)(oriNum * TF + (TF - TimeUtils.GetTimeS()));
        }

        // 添加个最大的时间戳
        public static double AddTimeFactorMin(double oriNum)
        {
            return (double)(oriNum * TF);
        }
        public static double AddTimeFactorMax(double oriNum)
        {
            return (double)(oriNum * TF + TF - 1);
        }

        public static double AddTimeFactor(double oriNum, int timestamp)
        {
            return (double)(oriNum * TF + (TF - timestamp));
        }

        public static double RemoveTimeFactor(double addTimeNum)
        {
            return Math.Floor(addTimeNum / TF);
        }

        //// 服务器使用
        public static DateTime Now()
        {
            return DateTime.UtcNow;
        }

        public static int GetTimeS()
        {
            return DateTimeToSeconds(Now());
        }

        public static long GetTimeMs()
        {
            return (long)(Now().Subtract(s_baseDate).TotalMilliseconds);
        }
    }
}