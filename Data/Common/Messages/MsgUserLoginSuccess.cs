using MessagePack;
using System.Collections.Generic;

namespace Data
{
    [MessagePackObject]
    public class MsgUserLoginSuccess
    {
        [Key(0)]
        public bool isNewUser;
        [Key(1)]
        public long userId;
        [Key(2)]
        public UserInfo? newUserInfo; // when isNewUser
    }

    [MessagePackObject]
    public class ResUserLoginSuccess
    {
        [Key(0)]
        public UserInfo userInfo;
        [Key(1)]
        public bool kickOther;
        [Key(2)]
        public int delayS;
    }
}