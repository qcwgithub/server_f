using MessagePack;
using System.Collections.Generic;

namespace Data
{
    [MessagePackObject]
    public class MsgUserLogin
    {
        [Key(0)]
        public string version;
        [Key(1)]
        public string platform;
        [Key(2)]
        public string channel;
        [Key(3)]
        public string channelUserId;
        [Key(4)]
        public string verifyData;
        [Key(5)]
        public long userId;
        [Key(6)]
        public string token;
        [Key(7)]
        public string deviceUid;
        [Key(8)]
        public Dictionary<string, string> dict;
    }

    [MessagePackObject]
    public class ResUserLogin
    {
        [Key(0)]
        public UserInfo userInfo;
        [Key(1)]
        public bool kickOther;
        [Key(2)]
        public int delayS;
    }
}