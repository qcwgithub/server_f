using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_SceneInfo_maxOf_roomId : Handler<DbService, MsgQuery_SceneInfo_maxOf_roomId, ResQuery_SceneInfo_maxOf_roomId>
    {
        public override MsgType msgType => MsgType._Query_SceneInfo_maxOf_roomId;

        public Query_SceneInfo_maxOf_roomId(Server server, DbService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgQuery_SceneInfo_maxOf_roomId msg, ResQuery_SceneInfo_maxOf_roomId res)
        {
            // this.service.logger.InfoFormat("{0}", this.msgType);

            var result = await this.service.collection_scene_info.Query_SceneInfo_maxOf_roomId();

            res.result = result;
            return ECode.Success;
        }
    }
}
