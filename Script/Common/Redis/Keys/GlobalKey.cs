namespace Script
{
    public static class GlobalKey
    {
        public static string TakeLockControl() => "global:takeLockControl";
        public static string LockedHash() => "global:lockedHash";
        public static string LockPrefix() => "lock:global";
        public static class LockKey
        {
            public static string GlobalInit() => LockPrefix() + ":init";
        }
    }
}