using Data;

namespace Script
{
    [AutoRegister]
    public class Search_SceneInfo : Handler<DbService, MsgSearch_SceneInfo, ResSearch_SceneInfo>
    {
        public Search_SceneInfo(Server server, DbService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Search_SceneInfo;
        public override async Task<ECode> Handle(MessageContext context, MsgSearch_SceneInfo msg, ResSearch_SceneInfo res)
        {
            this.service.logger.Info($"{this.msgType} keyword {msg.keyword}");

            res.sceneInfos = await this.service.collection_scene_info.Search(msg.keyword);
            return ECode.Success;
        }
    }
}