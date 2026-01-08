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

        #endregion auto
    }
}