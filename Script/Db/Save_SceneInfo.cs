using Data;

namespace Script
{
    [AutoRegister]
    public class Save_SceneInfo : Handler<DbService, MsgSave_SceneInfo, ResSave_SceneInfo>
    {
        public Save_SceneInfo(Server server, DbService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Save_SceneInfo;
        public override async Task<ECode> Handle(MessageContext context, MsgSave_SceneInfo msg, ResSave_SceneInfo res)
        {
            this.service.logger.InfoFormat("{0} roomIdId:{1}", this.msgType, msg.roomId);
            //MyResponse r = await this.service.table_player.Save(msg.playerId, msg.roomInfoNullable);

            ECode e = await this.service.collection_scene_info.Save(msg.roomId, msg.sceneInfoNullable);

#if DEBUG
            SceneInfo info_check = await this.service.collection_scene_info.Query_SceneInfo_by_roomId(msg.roomId);
            info_check.Ensure();
            if (!msg.sceneInfo_debug!.IsDifferent(info_check))
            {
                this.service.logger.Debug("--------Exact--------");
            }
            else
            {
                this.service.logger.Error("--------Different? ");

                msg.sceneInfo_debug.IsDifferent(info_check);
                msg.sceneInfo_debug.IsDifferent(info_check);
                msg.sceneInfo_debug.IsDifferent(info_check);
                msg.sceneInfo_debug.IsDifferent(info_check);
                msg.sceneInfo_debug.IsDifferent(info_check);
            }
#endif
            return e;
        }
    }
}