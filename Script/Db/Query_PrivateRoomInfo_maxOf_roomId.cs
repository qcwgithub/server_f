using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    [AutoRegister]
    public sealed class Query_PrivateRoomInfo_maxOf_roomId : Handler<DbService, MsgQuery_PrivateRoomInfo_maxOf_roomId, ResQuery_PrivateRoomInfo_maxOf_roomId>
    {
        public override MsgType msgType => MsgType._Query_PrivateRoomInfo_maxOf_roomId;

        public Query_PrivateRoomInfo_maxOf_roomId(Server server, DbService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgQuery_PrivateRoomInfo_maxOf_roomId msg, ResQuery_PrivateRoomInfo_maxOf_roomId res)
        {
            // this.service.logger.InfoFormat("{0}", this.msgType);

            var result = await this.service.collection_private_room_info.Query_PrivateRoomInfo_maxOf_roomId();

            res.result = result;
            return ECode.Success;
        }
    }
}
