using System.Threading.Tasks;
using Data;
using System.Diagnostics;
using System;


namespace Script
{
    public partial class DbService
    {
        async Task<(ECode, bool)> SaveFriendChatMessages(long roomId)
        {
            ChatMessage[] messages = await this.server.friendChatMessagesRedis.GetAll(roomId);
            await this.collection_friend_chat_message.Save(messages);
            return (ECode.Success, false);
        }
    }
}
