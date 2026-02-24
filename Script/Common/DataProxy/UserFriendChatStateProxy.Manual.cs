using System;
using System.Diagnostics;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Collections.Generic;
using Data;

namespace Script
{
    public partial class UserFriendChatStateProxy
    {
        const string USER_ID = "userId";
        const string MAX_SEQ = "maxSeq:";
        const string READ_SEQ = "readSeq:";
        const string UNREAD_COUNT = "unread:";

        protected override UserFriendChatState FromHashEntries(long userId, int p2, HashEntry[] entries)
        {
            var self = UserFriendChatState.Ensure(null);
            self.userId = userId;

            foreach (HashEntry entry in entries)
            {
                string name = entry.Name;
                if (name == USER_ID)
                {
                    MyDebug.Assert(userId == entry.Value);
                }
                else if (name.StartsWith(MAX_SEQ))
                {
                    long roomId = long.Parse(name.Substring(MAX_SEQ.Length));
                    long maxSeq = (long)entry.Value;

                    if (!self.roomDict.TryGetValue(roomId, out UserFriendChatStateRoom stateRoom))
                    {
                        stateRoom = UserFriendChatStateRoom.Ensure(null);
                        self.roomDict[roomId] = stateRoom;
                    }

                    MyDebug.Assert(stateRoom.maxSeq == 0);
                    stateRoom.maxSeq = maxSeq;
                }
                else if (name.StartsWith(READ_SEQ))
                {
                    long roomId = long.Parse(name.Substring(READ_SEQ.Length));
                    long readSeq = (long)entry.Value;

                    if (!self.roomDict.TryGetValue(roomId, out UserFriendChatStateRoom stateRoom))
                    {
                        stateRoom = UserFriendChatStateRoom.Ensure(null);
                        self.roomDict[roomId] = stateRoom;
                    }

                    MyDebug.Assert(stateRoom.readSeq == 0);
                    stateRoom.readSeq = readSeq;
                }
                else if (name.StartsWith(UNREAD_COUNT))
                {
                    long roomId = long.Parse(name.Substring(UNREAD_COUNT.Length));
                    long unreadCount = (long)entry.Value;

                    if (!self.roomDict.TryGetValue(roomId, out UserFriendChatStateRoom stateRoom))
                    {
                        stateRoom = UserFriendChatStateRoom.Ensure(null);
                        self.roomDict[roomId] = stateRoom;
                    }

                    MyDebug.Assert(stateRoom.unreadCount == 0);
                    stateRoom.unreadCount = unreadCount;
                }
                else
                {
                    throw new Exception($"UserFriendChatState.FromHashEntries bad name {name}");
                }
            }

            return self;
        }

        protected override HashEntry[] ToHashEntries(UserFriendChatState self)
        {
            List<HashEntry> entries = [];
            entries.Add(new HashEntry(USER_ID, self.userId));
            foreach (var kv in self.roomDict)
            {
                long roomId = kv.Key;
                UserFriendChatStateRoom stateRoom = kv.Value;
                if (stateRoom.maxSeq != 0)
                {
                    entries.Add(new HashEntry(MAX_SEQ + roomId, stateRoom.maxSeq));
                }
                if (stateRoom.readSeq != 0)
                {
                    entries.Add(new HashEntry(READ_SEQ + roomId, stateRoom.readSeq));
                }
                if (stateRoom.unreadCount != 0)
                {
                    entries.Add(new HashEntry(UNREAD_COUNT + roomId, stateRoom.unreadCount));
                }
            }
            return entries.ToArray();
        }

        public async Task IncreaseUnreadCount(long userId, long roomId, long inc)
        {
            MyDebug.Assert(inc > 0);
            await Task.WhenAll(
                this.GetDb().HashIncrementAsync(this.Key(userId), UNREAD_COUNT + roomId, inc),
                this.RPushToTaskQueue(userId, default));
        }

        public async Task SetMaxSeq(long userId, long roomId, long maxSeq)
        {
            MyDebug.Assert(maxSeq > 0);
            await Task.WhenAll(
                this.GetDb().HashSetAsync(this.Key(userId), MAX_SEQ + roomId, maxSeq),
                this.RPushToTaskQueue(userId, default));
        }

        async Task SetReadSeq(long userId, long roomId, long readSeq)
        {
            MyDebug.Assert(readSeq > 0);

            await Task.WhenAll(this.GetDb().HashSetAsync(this.Key(userId), READ_SEQ + roomId, readSeq),
                this.RPushToTaskQueue(userId, default));
        }

        public async Task SetReadSeq_DecreaseUnreadCount_1(long userId, long roomId, long readSeq, long? dec)
        {
            MyDebug.Assert(readSeq > 0);
            if (dec != null)
            {
                MyDebug.Assert(dec > 0);
            }

            RedisKey key = this.Key(userId);
            await Task.WhenAll(
                this.SetReadSeq(userId, roomId, readSeq),
                dec != null ? this.GetDb().HashDecrementAsync(key, UNREAD_COUNT + roomId, dec.Value) : Task.CompletedTask,
                this.RPushToTaskQueue(userId, default));
        }

        public async Task SetReadSeq_DecreaseUnreadCount_N(long userId, Dictionary<long, (long, long?)> dict)
        {
            var tasks = new List<Task>();
            foreach (var kv in dict)
            {
                long roomId = kv.Key;
                (long readSeq, long? dec) = kv.Value;

                tasks.Add(this.SetReadSeq(userId, roomId, readSeq));
                if (dec != null)
                {
                    tasks.Add(this.GetDb().HashDecrementAsync(this.Key(userId), UNREAD_COUNT + roomId, dec.Value));
                }
            }
            if (tasks.Count == 0)
            {
                return;
            }
            tasks.Add(this.RPushToTaskQueue(userId, default));
            await Task.WhenAll(tasks);
        }
    }
}
