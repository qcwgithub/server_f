using Data;

namespace Script
{
    [AutoRegister]
    public class Save_FriendChatRoomInfo : Handler<DbService, MsgSave_FriendChatRoomInfo, ResSave_FriendChatRoomInfo>
    {
        public Save_FriendChatRoomInfo(Server server, DbService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Save_FriendChatRoomInfo;
        public override async Task<ECode> Handle(MessageContext context, MsgSave_FriendChatRoomInfo msg, ResSave_FriendChatRoomInfo res)
        {
            this.service.logger.InfoFormat("{0} roomIdId:{1}", this.msgType, msg.roomId);
            //MyResponse r = await this.service.table_player.Save(msg.playerId, msg.roomInfoNullable);

            ECode e = await this.service.collection_friend_chat_room_info.Save(msg.roomId, msg.roomInfoNullable);

#if DEBUG
            FriendChatRoomInfo info_check = await this.service.collection_friend_chat_room_info.Query_FriendChatRoomInfo_by_roomId(msg.roomId);
            info_check.Ensure();
            if (!msg.roomInfo_debug!.IsDifferent(info_check))
            {
                this.service.logger.Debug("--------Exact--------");
            }
            else
            {
                this.service.logger.Error("--------Different? ");

                msg.roomInfo_debug.IsDifferent(info_check);
                msg.roomInfo_debug.IsDifferent(info_check);
                msg.roomInfo_debug.IsDifferent(info_check);
                msg.roomInfo_debug.IsDifferent(info_check);
                msg.roomInfo_debug.IsDifferent(info_check);
            }
#endif
            return e;
        }
    }
}