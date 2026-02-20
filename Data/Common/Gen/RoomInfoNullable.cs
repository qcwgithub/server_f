using System;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class RoomInfoNullable
    {
        #region auto

        [Key(0)]
        public long? roomId;
        [Key(1)]
        public long? createTimeS;
        [Key(2)]
        public string title;
        [Key(3)]
        public string desc;
        [Key(4)]
        public long? messageId;
        [Key(5)]
        public List<RoomParticipant> participants;

        #endregion auto
    }
}