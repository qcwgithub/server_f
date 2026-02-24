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
            ChatMessage[] messages = await this.server.friendChatMessagesInBoxRedis.GetAll(roomId);
            if (messages.Length > 0)
            {
                await this.collection_friend_chat_message.Save(messages);
                await this.server.friendChatMessagesInBoxRedis.Trim(roomId, messages.Length);
            }
            return (ECode.Success, false);
        }
    }
}
