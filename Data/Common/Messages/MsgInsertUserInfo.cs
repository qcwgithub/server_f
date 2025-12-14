using System.Collections.Generic;
using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgInsertUserInfo
    {
        [Key(0)]
        public UserInfo userInfo;
    }

    [MessagePackObject]
    public class ResInsertUserInfo
    {

    }
}