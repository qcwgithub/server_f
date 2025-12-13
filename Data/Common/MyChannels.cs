namespace Data
{
    public static class MyChannels
    {
        public const string uuid = "uuid";

        public static bool IsValidChannel(string channel)
        {
            return channel == MyChannels.uuid;
        }
    }
}