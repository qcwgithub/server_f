using Data;

namespace Script
{
    public class Insert_SceneInfo : Handler<DbService, MsgInsert_SceneInfo, ResInsert_SceneInfo>
    {
        public Insert_SceneInfo(Server server, DbService service) : base(server, service)
        {
        }


        public override MsgType msgType => MsgType._Insert_SceneInfo;
        public override async Task<ECode> Handle(MessageContext context, MsgInsert_SceneInfo msg, ResInsert_SceneInfo res)
        {
            this.service.logger.InfoFormat("{0}, roomId: {1}", this.msgType, msg.sceneInfo.sceneId);

            await this.service.collection_scene_info.Insert(msg.sceneInfo);
            return ECode.Success;
        }
    }
}