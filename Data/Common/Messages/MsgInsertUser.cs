using System.Collections.Generic;
using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgInsertUser
    {
        [Key(0)]
        public long userId;
        
        [Key(1)]
        public Profile profile;
    }    
}