using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    [AutoRegister]
    public sealed class Save_UserFriendChatState : Handler<DbService, MsgSave_UserFriendChatState, ResSave_UserFriendChatState>
    {
        public override MsgType msgType => MsgType._Save_UserFriendChatState;

        public Save_UserFriendChatState(Server server, DbService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgSave_UserFriendChatState msg, ResSave_UserFriendChatState res)
        {
            this.service.logger.InfoFormat("{0} userId {1}", this.msgType, msg.info.userId);

            ECode e = await this.service.collection_user_friend_chat_state.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            return ECode.Success;
        }
    }
}
