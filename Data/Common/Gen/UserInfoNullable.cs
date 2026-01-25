using System;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class UserInfoNullable
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
        [Key(4)]
        public long? lastSetNameTimeS;
        [Key(5)]
        public int? avatarIndex;
        [Key(6)]
        public long? lastSetAvatarIndexTimeS;

        #endregion auto
    }
}