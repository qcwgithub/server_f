using Data;

namespace Script
{
    public class Db_QueryUserInfo : Handler<DbService, MsgQueryUserInfo, ResQueryUserInfo>
    {
        public Db_QueryUserInfo(Server server, DbService service) : base(server, service)
        {
        }


        public override MsgType msgType => MsgType._Db_QueryUserInfo;
        public override async Task<ECode> Handle(IConnection connection, MsgQueryUserInfo msg, ResQueryUserInfo res)
        {
            res.userInfo = await this.service.collection_user_info.Query(msg.userId);
            return ECode.Success;
        }
    }
}