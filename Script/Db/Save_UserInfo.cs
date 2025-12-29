using Data;

namespace Script
{
    public class Save_UserInfo : Handler<DbService, MsgSave_UserInfo, ResSave_UserInfo>
    {
        public Save_UserInfo(Server server, DbService service) : base(server, service)
        {
        }


        public override MsgType msgType => MsgType._Save_UserInfo;
        public override async Task<ECode> Handle(MsgContext context, MsgSave_UserInfo msg, ResSave_UserInfo res)
        {
            this.service.logger.InfoFormat("{0} userIdId:{1}", this.msgType, msg.userId);
            //MyResponse r = await this.service.table_player.Save(msg.playerId, msg.userInfoNullable);

            ECode e = await this.service.collection_user_info.Save(msg.userId, msg.userInfoNullable);

#if DEBUG
            UserInfo info_check = await this.service.collection_user_info.Query_UserInfo_by_userId(msg.userId);
            info_check.Ensure();
            if (!msg.userInfo_debug!.IsDifferent(info_check))
            {
                this.service.logger.Debug("--------Exact--------");
            }
            else
            {
                this.service.logger.Error("--------Different? ");

                msg.userInfo_debug.IsDifferent(info_check);
                msg.userInfo_debug.IsDifferent(info_check);
                msg.userInfo_debug.IsDifferent(info_check);
                msg.userInfo_debug.IsDifferent(info_check);
                msg.userInfo_debug.IsDifferent(info_check);
            }
#endif
            return e;
        }
    }
}