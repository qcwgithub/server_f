using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_SceneInfo_maxOf_sceneId : Handler<DbService, MsgQuery_SceneInfo_maxOf_sceneId, ResQuery_SceneInfo_maxOf_sceneId>
    {
        public override MsgType msgType => MsgType._Query_SceneInfo_maxOf_sceneId;

        public Query_SceneInfo_maxOf_sceneId(Server server, DbService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgQuery_SceneInfo_maxOf_sceneId msg, ResQuery_SceneInfo_maxOf_sceneId res)
        {
            // this.service.logger.InfoFormat("{0}", this.msgType);

            var result = await this.service.collection_scene_info.Query_SceneInfo_maxOf_sceneId();

            res.result = result;
            return ECode.Success;
        }
    }
}
