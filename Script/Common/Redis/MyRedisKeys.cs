using System;
using System.Linq;
using System.Threading.Tasks;
using Data;


namespace Script
{
    public static class CommonKey
    {

    }

    public static class GlobalKey
    {
        public static string TakeLockControl() => "global:takeLockControl";
        public static string LockedHash() => "global:lockedHash";
        public static string LockPrefix() => "lock:global";
        public static class LockKey
        {
            public static string GlobalInit() => LockPrefix() + ":init";
            public static string UpdateSubRanks() => LockPrefix() + ":updateSubRanks";
        }

        public static string MaxPlayerId(int serverId) => $"s{serverId}:global:maxPlayerId";
        public static string MaxPlayerIdInitedFlag(int serverId) => $"s{serverId}:global:maxPlayerIdInited";

        public static string PlayerNamesInitedFlag(int serverId) => $"s{serverId}:global:playerNamesInited";
    }

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

    public class TaskQueueOwner
    {
        public int serviceId;

        // 下线超过 1 分钟，所管理的 taskQueue 会被其他人抢走
        public long offlineTimeS;
    }

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

    public static class AccountKey
    {
        public static string AccountInfo(string channel, string channelUserId) => "account:" + channel + ":" + channelUserId;
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

    public static class LockKey
    {
        public static string Account(string channel, string channelUserId) => "lock:account:" + channel + ":" + channelUserId;

        // 旧的锁控制
        // 锁，为了从 mysql 加载数据到 redis
        public static class LoadDataFromDBToRedis
        {
            
        }
    }

    public static class UserKey
    {
        public static string Brief(long userId) => "user:" + userId + ":brief";

        public static string PSId(long userId) => "user:" + userId + ":psId";
    }

    public static class NameKey
    {
        public static string UserName(int serverId, string base64Name) => $"s{serverId}:name:user:" + base64Name;
    }
}