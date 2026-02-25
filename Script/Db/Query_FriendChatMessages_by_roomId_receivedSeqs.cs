using Data;
using System.Threading.Tasks;

namespace Script
{
    [AutoRegister]
    public sealed class Query_FriendChatMessages_by_roomId_receivedSeqs : Handler<DbService, MsgQuery_FriendChatMessages_by_roomId_receivedSeqs, ResQuery_FriendChatMessages_by_roomId_receivedSeqs>
    {
        public override MsgType msgType => MsgType._Query_FriendChatMessages_by_roomId_receivedSeqs;

        public Query_FriendChatMessages_by_roomId_receivedSeqs(Server server, DbService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgQuery_FriendChatMessages_by_roomId_receivedSeqs msg, ResQuery_FriendChatMessages_by_roomId_receivedSeqs res)
        {
            // this.service.logger.InfoFormat("{0} userId: {1}", this.msgType, msg.userId);

            var result = await this.service.collection_friend_chat_message.Query_FriendChatMessages_by_roomId_receivedSeqs(msg.roomIdToReceivedSeqs);

            res.messages = result;
            return ECode.Success;
        }
    }
}
