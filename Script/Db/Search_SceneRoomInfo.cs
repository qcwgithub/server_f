using Data;

namespace Script
{
    public class Search_SceneRoomInfo : Handler<DbService, MsgSearch_SceneRoomInfo, ResSearch_SceneRoomInfo>
    {
        public Search_SceneRoomInfo(Server server, DbService service) : base(server, service)
        {
        }


        public override MsgType msgType => MsgType._Search_SceneRoomInfo;
        public override async Task<ECode> Handle(MessageContext context, MsgSearch_SceneRoomInfo msg, ResSearch_SceneRoomInfo res)
        {
            this.service.logger.Info($"{this.msgType} keyword {msg.keyword}");

            res.sceneRoomInfos = await this.service.collection_scene_room_info.Search(msg.keyword);
            return ECode.Success;
        }
    }
}