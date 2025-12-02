using System.Collections.Generic;
using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgDestroyUser
    {
        [Key(0)]
        public long playerId;
        [Key(1)]
        public string place;
        [Key(2)]
        public MsgKick msgKick;

        public static MsgDestroyUser Create(long playerId, string place, MsgKick msgKick)
        {
            var self = new MsgDestroyUser();
            self.playerId = playerId;
            self.place = place;
            self.msgKick = msgKick;
            return self;
        }
    }
}