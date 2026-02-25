using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    [AutoRegister]
    public sealed class Query_SceneRoomInfo_maxOf_roomId : Handler<DbService, MsgQuery_SceneRoomInfo_maxOf_roomId, ResQuery_SceneRoomInfo_maxOf_roomId>
    {
        public override MsgType msgType => MsgType._Query_SceneRoomInfo_maxOf_roomId;

        public Query_SceneRoomInfo_maxOf_roomId(Server server, DbService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgQuery_SceneRoomInfo_maxOf_roomId msg, ResQuery_SceneRoomInfo_maxOf_roomId res)
        {
            // this.service.logger.InfoFormat("{0}", this.msgType);

            var result = await this.service.collection_scene_room_info.Query_SceneRoomInfo_maxOf_roomId();

            res.result = result;
            return ECode.Success;
        }
    }
}
