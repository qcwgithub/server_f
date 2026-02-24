using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    [AutoRegister]
    public sealed class Query_UserFriendChatState_by_userId : Handler<DbService, MsgQuery_UserFriendChatState_by_userId, ResQuery_UserFriendChatState_by_userId>
    {
        public override MsgType msgType => MsgType._Query_UserFriendChatState_by_userId;

        public Query_UserFriendChatState_by_userId(Server server, DbService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgQuery_UserFriendChatState_by_userId msg, ResQuery_UserFriendChatState_by_userId res)
        {
            // this.service.logger.InfoFormat("{0} userId: {1}", this.msgType, msg.userId);

            var result = await this.service.collection_user_friend_chat_state.Query_UserFriendChatState_by_userId(msg.userId);

            res.result = result;
            return ECode.Success;
        }
    }
}
