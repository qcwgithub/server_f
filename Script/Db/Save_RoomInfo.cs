using Data;

namespace Script
{
    public class Save_RoomInfo : Handler<DbService, MsgSave_RoomInfo, ResSave_RoomInfo>
    {
        public Save_RoomInfo(Server server, DbService service) : base(server, service)
        {
        }


        public override MsgType msgType => MsgType._Save_RoomInfo;
        public override async Task<ECode> Handle(MessageContext context, MsgSave_RoomInfo msg, ResSave_RoomInfo res)
        {
            this.service.logger.InfoFormat("{0} roomIdId:{1}", this.msgType, msg.roomId);
            //MyResponse r = await this.service.table_player.Save(msg.playerId, msg.roomInfoNullable);

            ECode e = await this.service.collection_room_info.Save(msg.roomId, msg.roomInfoNullable);

#if DEBUG
            RoomInfo info_check = await this.service.collection_room_info.Query_RoomInfo_by_roomId(msg.roomId);
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