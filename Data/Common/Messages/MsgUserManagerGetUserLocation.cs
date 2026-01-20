using MessagePack;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Data
{
    [MessagePackObject]
    public class MsgUserManagerGetUserLocation
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public bool addWhenNotExist;
        [Key(2)]
        public string channel; // used by UserManager
        [Key(3)]
        public string channelUserId; // used by UserManager
    }

    [MessagePackObject]
    public class ResUserManagerGetUserLocation
    {
        [Key(0)]
        public stObjectLocation location;
    }
}