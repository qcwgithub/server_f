using System;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class FriendChatInfoInfoNullable
    {
        #region auto

        [Key(0)]
        public long? roomId;
        [Key(1)]
        public long? createTimeS;
        [Key(2)]
        public long? messageSeq;
        [Key(3)]
        public List<PrivateRoomUser> users;

        #endregion auto
    }
}