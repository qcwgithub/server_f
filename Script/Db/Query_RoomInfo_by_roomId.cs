using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_RoomInfo_by_roomId : Handler<DbService, MsgQuery_RoomInfo_by_roomId, ResQuery_RoomInfo_by_roomId>
    {
        public override MsgType msgType => MsgType._Query_RoomInfo_by_roomId;

        public Query_RoomInfo_by_roomId(Server server, DbService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(IConnection connection, MsgQuery_RoomInfo_by_roomId msg, ResQuery_RoomInfo_by_roomId res)
        {
            // this.service.logger.InfoFormat("{0} roomId: {1}", this.msgType, msg.roomId);

            var result = await this.service.collection_room_info.Query_RoomInfo_by_roomId(msg.roomId);

            res.result = result;
            return ECode.Success;
        }
    }
}
