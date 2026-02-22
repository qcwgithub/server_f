using System;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class PrivateRoomInfoNullable
    {
        #region auto

        [Key(0)]
        public long? roomId;
        [Key(1)]
        public long? createTimeS;
        [Key(2)]
        public long? seq;
        [Key(3)]
        public List<PrivateRoomUser> users;

        #endregion auto
    }
}