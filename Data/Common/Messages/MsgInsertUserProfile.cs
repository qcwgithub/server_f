using System.Collections.Generic;
using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgInsertUserProfile
    {
        [Key(0)]
        public Profile profile;
    }    
}