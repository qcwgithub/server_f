using Data;

namespace Script
{
    public class Db_SaveUserProfile : Handler<DbService, MsgSaveUserProfile, ResSaveUserProfile>
    {
        public override MsgType msgType => MsgType._Db_SaveUserProfile;
        public override async Task<ECode> Handle(ProtocolClientData socket, MsgSaveUserProfile msg, ResSaveUserProfile res)
        {
            this.service.logger.InfoFormat("{0} userIdId:{1}", this.msgType, msg.userId);
            //MyResponse r = await this.service.table_player.Save(msg.playerId, msg.profileNullable);

            ECode e = await this.service.collection_user_profile.Save(msg.userId, msg.profileNullable);

#if DEBUG
            Profile profile_check = await this.service.collection_user_profile.Query(msg.userId);
            profile_check.Ensure();
            if (!msg.profile_debug!.IsDifferent(profile_check))
            {
                this.service.logger.Debug("--------Exact--------");
            }
            else
            {
                this.service.logger.Error("--------Different? ");

                msg.profile_debug.IsDifferent(profile_check);
                msg.profile_debug.IsDifferent(profile_check);
                msg.profile_debug.IsDifferent(profile_check);
                msg.profile_debug.IsDifferent(profile_check);
                msg.profile_debug.IsDifferent(profile_check);
            }
#endif
            return e;
        }
    }
}