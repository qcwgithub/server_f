using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_RoomInfo_maxOf_roomId : Handler<DbService, MsgQuery_RoomInfo_maxOf_roomId, ResQuery_RoomInfo_maxOf_roomId>
    {
        public override MsgType msgType => MsgType._Query_RoomInfo_maxOf_roomId;

        public Query_RoomInfo_maxOf_roomId(Server server, DbService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgQuery_RoomInfo_maxOf_roomId msg, ResQuery_RoomInfo_maxOf_roomId res)
        {
            // this.service.logger.InfoFormat("{0}", this.msgType);

            var result = await this.service.collection_room_info.Query_RoomInfo_maxOf_roomId();

            res.result = result;
            return ECode.Success;
        }
    }
}
