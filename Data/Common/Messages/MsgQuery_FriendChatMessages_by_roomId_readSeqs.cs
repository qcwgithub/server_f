using System.Collections.Generic;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public sealed class MsgQuery_FriendChatMessages_by_roomId_readSeqs
    {
        [Key(0)]
        public Dictionary<long, long> roomIdToReadSeqs;
    }
    
    [MessagePackObject]
    public sealed class ResQuery_FriendChatMessages_by_roomId_readSeqs
    {
        [Key(0)]
        public List<ChatMessage> messages;
    }
}
