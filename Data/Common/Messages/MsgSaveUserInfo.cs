using System.Collections.Generic;
using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgSaveUserInfo
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        required public UserInfoNullable userInfoNullable;
        [Key(2)]
        public UserInfo? userInfo_debug;
    }

    [MessagePackObject]
    public class ResSaveUserInfo
    {
    }
}