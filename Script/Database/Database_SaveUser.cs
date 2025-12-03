using Data;

namespace Script
{
    public class Database_SaveUser : Handler<DatabaseService>
    {
        public override MsgType msgType => MsgType._Database_SaveUser;
        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgDatabaseSaveUser>(_msg);
            this.service.logger.InfoFormat("{0} userIdId:{1}", this.msgType, msg.userId);
            //MyResponse r = await this.service.table_player.Save(msg.playerId, msg.profileNullable);

            MyResponse r = await this.service.collection_user.Save(msg.userId, msg.profileNullable);

#if DEBUG
            Profile profile_check = (await this.service.collection_user.QueryById(msg.userId)).CastRes<List<Profile>>()[0];
            profile_check.Ensure();
            if (!msg.profile_debug.IsDifferent(profile_check))
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
            return r;
        }
    }
}