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

        #endregion auto
    }
}