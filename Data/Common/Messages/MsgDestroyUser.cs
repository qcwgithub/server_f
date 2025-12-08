using System.Collections.Generic;
using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgDestroyUser
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public string place;
        [Key(2)]
        public MsgKick msgKick;

        public static MsgDestroyUser Create(long playerId, string place, MsgKick msgKick)
        {
            var self = new MsgDestroyUser();
            self.userId = playerId;
            self.place = place;
            self.msgKick = msgKick;
            return self;
        }
    }

    [MessagePackObject]
    public class ResDestroyUser
    {

    }
}