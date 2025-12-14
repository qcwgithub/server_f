using System.Collections.Generic;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgQueryUserInfo
    {
        [Key(0)]
        public long userId;
    }

    [MessagePackObject]
    public class ResQueryUserInfo
    {
        [Key(0)]
        public UserInfo? userInfo;
    }
}