using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    [AutoRegister]
    public sealed class Query_SceneInfo_by_roomId : Handler<DbService, MsgQuery_SceneInfo_by_roomId, ResQuery_SceneInfo_by_roomId>
    {
        public override MsgType msgType => MsgType._Query_SceneInfo_by_roomId;

        public Query_SceneInfo_by_roomId(Server server, DbService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgQuery_SceneInfo_by_roomId msg, ResQuery_SceneInfo_by_roomId res)
        {
            // this.service.logger.InfoFormat("{0} roomId: {1}", this.msgType, msg.roomId);

            var result = await this.service.collection_scene_info.Query_SceneInfo_by_roomId(msg.roomId);

            res.result = result;
            return ECode.Success;
        }
    }
}
