using System.Collections.Generic;
using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgSave_UserInfo
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        required public UserInfoNullable userInfoNullable;
        [Key(2)]
        public UserInfo? userInfo_debug;
    }

    [MessagePackObject]
    public class ResSave_UserInfo
    {
    }
}