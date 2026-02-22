using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    [AutoRegister]
    public sealed class Query_FriendChatInfo_maxOf_roomId : Handler<DbService, MsgQuery_FriendChatInfo_maxOf_roomId, ResQuery_FriendChatInfo_maxOf_roomId>
    {
        public override MsgType msgType => MsgType._Query_FriendChatInfo_maxOf_roomId;

        public Query_FriendChatInfo_maxOf_roomId(Server server, DbService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgQuery_FriendChatInfo_maxOf_roomId msg, ResQuery_FriendChatInfo_maxOf_roomId res)
        {
            // this.service.logger.InfoFormat("{0}", this.msgType);

            var result = await this.service.collection_friend_chat_info.Query_FriendChatInfo_maxOf_roomId();

            res.result = result;
            return ECode.Success;
        }
    }
}
