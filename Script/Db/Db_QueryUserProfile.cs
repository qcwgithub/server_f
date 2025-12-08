using Data;

namespace Script
{
    public class Db_QueryUserProfile : Handler<DbService, MsgQueryUserProfile, ResQueryUserProfile>
    {
        public override MsgType msgType => MsgType._Db_QueryUserProfile;
        public override async Task<ECode> Handle(ProtocolClientData socket, MsgQueryUserProfile msg, ResQueryUserProfile res)
        {
            res.profile = await this.service.collection_user_profile.Query(msg.userId);
            return ECode.Success;
        }
    }
}