using System.Collections.Generic;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public sealed class MsgQuery_FriendChatMessages_by_roomId_receivedSeqs
    {
        [Key(0)]
        public Dictionary<long, long> roomIdToReceivedSeqs;
    }
    
    [MessagePackObject]
    public sealed class ResQuery_FriendChatMessages_by_roomId_receivedSeqs
    {
        [Key(0)]
        public List<ChatMessage> messages;
    }
}
