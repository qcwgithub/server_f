namespace Script
{
    public static class GGlobalKey
    {
        public static string TakeLockControl() => "gglobal:takeLockControl";
        public static string LockedHash() => "gglobal:lockedHash";
        public static string LockPrefix() => "lock:gglobal";
        public static class LockKey
        {
            public static string GlobalInit() => LockPrefix() + ":init";
        }
    }

    public static class AAAKey
    {
        public static string TakeLockControl() => "aaa:takeLockControl";
        public static string LockedHash() => "aaa:lockedHash";
        public static string LockPrefix() => "lock:aaa";
        public static class LockKey
        {
            public static string Tick() => LockPrefix() + ":tick";
        }

        public static string TaskPassInfo() => "aaa:taskPassInfo";
        public static string RadarPassInfo() => "aaa:radarPassInfo";
    }

    public static class GAAAKey
    {
        public static string TakeLockControl() => "gaaa:takeLockControl";
        public static string LockedHash() => "gaaa:lockedHash";
        public static string LockPrefix() => "lock:gaaa";
        public static class LockKey
        {
            // public static string Tick() => LockPrefix() + ":tick";
            public static string RD20250409() => LockPrefix() + ":RD20250409";
        }

        // public static string TaskPassInfo() => "aaa:taskPassInfo";
        public static string ForbidAccount(string channelUserId)
        {
            return $"gaaa:forbidAccount:{channelUserId}";
        }
    }



    public static class NameKey
    {
        public static string UserName(int serverId, string base64Name) => $"s{serverId}:name:user:" + base64Name;
    }
}