using Data;

namespace Script
{
    public class Insert_RoomInfo : Handler<DbService, MsgInsert_RoomInfo, ResInsert_RoomInfo>
    {
        public Insert_RoomInfo(Server server, DbService service) : base(server, service)
        {
        }


        public override MsgType msgType => MsgType._Insert_RoomInfo;
        public override async Task<ECode> Handle(MessageContext context, MsgInsert_RoomInfo msg, ResInsert_RoomInfo res)
        {
            this.service.logger.InfoFormat("{0}, roomId: {1}", this.msgType, msg.roomInfo.roomId);

            await this.service.collection_room_info.Insert(msg.roomInfo);
            return ECode.Success;
        }
    }
}