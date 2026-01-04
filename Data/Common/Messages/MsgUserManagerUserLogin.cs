using MessagePack;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Data
{
    [MessagePackObject]
    public class MsgUserManagerUserLogin
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
        public string token;
        [Key(6)]
        public string deviceUid;
        [Key(7)]
        public string addressFamily;
        [Key(8)]
        public string ip;
    }

    [MessagePackObject]
    public class ResUserManagerUserLogin
    {
        [Key(0)]
        public bool isNewUser;
        [Key(1)]
        public UserInfo userInfo;
        [Key(2)]
        public bool kickOther;
        [Key(3)]
        public int userServiceId;
    }
}