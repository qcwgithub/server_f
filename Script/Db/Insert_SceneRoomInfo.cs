using Data;

namespace Script
{
    public class Insert_SceneRoomInfo : Handler<DbService, MsgInsert_SceneRoomInfo, ResInsert_SceneRoomInfo>
    {
        public Insert_SceneRoomInfo(Server server, DbService service) : base(server, service)
        {
        }


        public override MsgType msgType => MsgType._Insert_SceneRoomInfo;
        public override async Task<ECode> Handle(MessageContext context, MsgInsert_SceneRoomInfo msg, ResInsert_SceneRoomInfo res)
        {
            this.service.logger.InfoFormat("{0}, roomId: {1}", this.msgType, msg.sceneRoomInfo.roomId);

            await this.service.collection_scene_room_info.Insert(msg.sceneRoomInfo);
            return ECode.Success;
        }
    }
}