
using System;
using MessagePack;
using System.Numerics;

namespace Data
{
    [Flags]
    public enum LoadPlayerNewestWhat
    {
        None = 0,
        Profile = 1, // Command 使用
    }

    [MessagePackObject]
    public class PlayerNewestInfo
    {
        [Key(0)]
        public long playerId;
        [Key(1)]
        public Profile profile;

        public static PlayerNewestInfo Create(long playerId)
        {
            var self = new PlayerNewestInfo();
            self.playerId = playerId;
            return self;
        }
    }
}