using Data;

namespace Script
{
    public class Db_SaveUserProfile : Handler<DbService, MsgSaveUserInfo, ResSaveUserInfo>
    {
        public Db_SaveUserProfile(Server server, DbService service) : base(server, service)
        {
        }


        public override MsgType msgType => MsgType._Db_SaveUserInfo;
        public override async Task<ECode> Handle(IConnection connection, MsgSaveUserInfo msg, ResSaveUserInfo res)
        {
            this.service.logger.InfoFormat("{0} userIdId:{1}", this.msgType, msg.userId);
            //MyResponse r = await this.service.table_player.Save(msg.playerId, msg.profileNullable);

            ECode e = await this.service.collection_user_info.Save(msg.userId, msg.userInfoNullable);

#if DEBUG
            UserInfo info_check = await this.service.collection_user_info.Query(msg.userId);
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