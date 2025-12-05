using System.Collections.Generic;
using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgSaveUserProfile
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public ProfileNullable? profileNullable;
        [Key(2)]
        public Profile? profile_debug;
    }    
}