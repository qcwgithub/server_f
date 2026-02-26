using Data;

namespace Script
{
    [AutoRegister]
    public class Insert_FriendChatRoomInfo : Handler<DbService, MsgInsert_FriendChatRoomInfo, ResInsert_FriendChatRoomInfo>
    {
        public Insert_FriendChatRoomInfo(Server server, DbService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Insert_FriendChatRoomInfo;
        public override async Task<ECode> Handle(MessageContext context, MsgInsert_FriendChatRoomInfo msg, ResInsert_FriendChatRoomInfo res)
        {
            this.service.logger.InfoFormat("{0}, roomId: {1}", this.msgType, msg.roomInfo.roomId);

            await this.service.collection_friend_chat_room_info.Insert(msg.roomInfo);
            return ECode.Success;
        }
    }
}