using Data;

namespace Script
{
    public class Db_InsertUserProfile : Handler<DbService, MsgInsertUserProfile, ResInsertUserProfile>
    {
        public override MsgType msgType => MsgType._Db_InsertUserProfile;
        public override async Task<ECode> Handle(ProtocolClientData socket, MsgInsertUserProfile msg, ResInsertUserProfile res)
        {
            this.service.logger.InfoFormat("{0}, userId: {1}", this.msgType, msg.profile.userId);

            await this.service.collection_user_profile.Insert(msg.profile);
            return ECode.Success;
        }
    }
}