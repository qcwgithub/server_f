namespace Script
{
    public static class DbKey
    {
        public static string TakeLockControl() => "db:takeLockControl";
        public static string LockedHash() => "db:lockedHash";
        public static string LockPrefix() => "lock:db";
        public static class LockKey
        {
            public static string AssignPersistenceTaskQueueOwners() => LockPrefix() + ":assignPersistenceTaskQueueOwners";
        }

        // list of stDirtyElementWithTime.sToString()
        public static string PersistenceTaskQueueList(int queue) => "db:persistenceTaskQueueList:" + queue;
        // member = stDirtyElement
        public static string PersistenceTaskQueueSortedSet(int queue) => "db:persistenceTaskQueueSortedSet:" + queue;

        // hash of (taskQueue -> TaskQueueOwner)
        public static string PersistenceTaskQueueOwners() => "db:persistenceTaskQueueOwners";
    }
}