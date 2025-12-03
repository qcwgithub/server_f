
using System;
using MessagePack;
using System.Numerics;

namespace Data
{
    [Flags]
    public enum LoadUserNewestWhat
    {
        None = 0,
        Profile = 1, // Command 使用
    }

    [MessagePackObject]
    public class UserNewestInfo
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public Profile? profile;

        public static UserNewestInfo Create(long userId)
        {
            var self = new UserNewestInfo();
            self.userId = userId;
            return self;
        }
    }
}