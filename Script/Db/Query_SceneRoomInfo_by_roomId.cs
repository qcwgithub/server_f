using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_SceneRoomInfo_by_roomId : Handler<DbService, MsgQuery_SceneRoomInfo_by_roomId, ResQuery_SceneRoomInfo_by_roomId>
    {
        public override MsgType msgType => MsgType._Query_SceneRoomInfo_by_roomId;

        public Query_SceneRoomInfo_by_roomId(Server server, DbService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgQuery_SceneRoomInfo_by_roomId msg, ResQuery_SceneRoomInfo_by_roomId res)
        {
            // this.service.logger.InfoFormat("{0} roomId: {1}", this.msgType, msg.roomId);

            var result = await this.service.collection_scene_room_info.Query_SceneRoomInfo_by_roomId(msg.roomId);

            res.result = result;
            return ECode.Success;
        }
    }
}
