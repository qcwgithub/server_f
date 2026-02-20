using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_SceneInfo_by_sceneId : Handler<DbService, MsgQuery_SceneInfo_by_sceneId, ResQuery_SceneInfo_by_sceneId>
    {
        public override MsgType msgType => MsgType._Query_SceneInfo_by_sceneId;

        public Query_SceneInfo_by_sceneId(Server server, DbService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgQuery_SceneInfo_by_sceneId msg, ResQuery_SceneInfo_by_sceneId res)
        {
            // this.service.logger.InfoFormat("{0} sceneId: {1}", this.msgType, msg.sceneId);

            var result = await this.service.collection_scene_info.Query_SceneInfo_by_sceneId(msg.sceneId);

            res.result = result;
            return ECode.Success;
        }
    }
}
