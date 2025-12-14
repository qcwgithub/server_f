using System.Collections.Generic;
using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgInsert_UserInfo
    {
        [Key(0)]
        public UserInfo userInfo;
    }

    [MessagePackObject]
    public class ResInsert_UserInfo
    {

    }
}