using Data;

namespace Script
{
    public class Db_QueryUserProfile : Handler<DbService, MsgQueryUserProfile>
    {
        public override MsgType msgType => MsgType._Db_QueryUserProfile;
        public override async Task<MyResponse> Handle(ProtocolClientData socket, MsgQueryUserProfile msg)
        {
            MyResponse r = await this.service.collection_user_profile.Query(msg.userId);
            if (r.err != ECode.Success)
            {
                return r;
            }

            var res = new ResQueryUserProfile();
            res.profile = r.CastRes<Profile>();
            return new MyResponse(ECode.Success, res);
        }
    }
}