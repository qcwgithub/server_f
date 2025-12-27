namespace Script
{
    public static class LockKey
    {
        public static string Account(string channel, string channelUserId) => "lock:account:" + channel + ":" + channelUserId;
        public static string Room(long roomId) => "lock:room:" + roomId;

        // 旧的锁控制
        // 锁，为了从 mongodb 加载数据到 redis
        public static class LoadDataFromDBToRedis
        {
            public static string AccountInfo(string channel, string channelUserId) => "lock:mongoLoad:accountInfo:" + channel + ":" + channelUserId;
        }
    }
}