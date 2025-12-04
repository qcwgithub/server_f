using System;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class ProfileNullable
    {
        #region auto

        [Key(0)]
        public long? userId;
        [Key(1)]
        public string userName;
        [Key(2)]
        public long? createTimeS;
        [Key(3)]
        public long? lastLoginTimeS;

        #endregion auto
    }
}