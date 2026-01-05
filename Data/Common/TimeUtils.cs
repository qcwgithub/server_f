namespace Data
{
    public static class TimeUtils
    {
        public static readonly DateTime s_baseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        static void AssertIsUtc(DateTime dt)
        {
            if (dt.Kind != DateTimeKind.Utc)
            {
                ConsoleColor old = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("dt.Kind != Utc");
                Console.ForegroundColor = old;
            }
        }

        public static DateTime GetDateTime()
        {
            return DateTime.UtcNow;
        }

        public static DateTime SecondsToDateTime(long s)
        {
            DateTime dt = s_baseDate.AddSeconds(s);
            AssertIsUtc(dt);
            return dt;
        }

        public static long DateTimeToSeconds(DateTime dt)
        {
            AssertIsUtc(dt);
            return (long)(dt - s_baseDate).TotalSeconds;
        }

        public static DateTime MillisecondsToDateTime(long s)
        {
            DateTime dt = s_baseDate.AddMilliseconds(s);
            AssertIsUtc(dt);
            return dt;
        }

        public static long GetTimeS()
        {
            return DateTimeToSeconds(GetDateTime());
        }

        public static long DateTimeToMilliseconds(DateTime dt)
        {
            AssertIsUtc(dt);
            return (long)(dt - s_baseDate).TotalMilliseconds;
        }

        public static DateTime CreateDateTime(int year, int month, int day)
        {
            return new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
        }

        public static long GetTime()
        {
            return DateTimeToMilliseconds(GetDateTime());
        }
    }
}