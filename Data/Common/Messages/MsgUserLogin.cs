using MessagePack;
using System.Collections.Generic;

namespace Data
{
    [MessagePackObject]
    public class MsgUserLogin
    {
        [Key(0)]
        public bool isReconnect;

        [Key(1)]
        public long userId;

        [Key(2)]
        public string token;

        [Key(3)]
        public string language;

        [Key(4)]
        public long timeMs; // 客户端时间
    }

    [MessagePackObject]
    public class ServerTimeInfo
    {
        [Key(0)]
        public int refreshClock;
        [Key(1)]
        public long timeMs;
        [Key(2)]
        public int timezoneOffset;
        [Key(3)]
        public int debugTimeOffsetS;
    }

    [MessagePackObject]
    public class ResUserLogin
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public Profile profile;
        [Key(2)]
        public bool isNewProfile;
        [Key(3)]
        public bool kickOther; // 顶号
    }
}