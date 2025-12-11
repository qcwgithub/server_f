using Data;

namespace Script
{
    public class Db_QueryUserProfile : Handler<DbService, MsgQueryUserProfile, ResQueryUserProfile>
    {
        public Db_QueryUserProfile(Server server, DbService service) : base(server, service)
        {
        }


        public override MsgType msgType => MsgType._Db_QueryUserProfile;
        public override async Task<ECode> Handle(IConnection connection, MsgQueryUserProfile msg, ResQueryUserProfile res)
        {
            res.profile = await this.service.collection_user_profile.Query(msg.userId);
            return ECode.Success;
        }
    }
}