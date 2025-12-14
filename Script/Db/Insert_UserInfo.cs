using Data;

namespace Script
{
    public class Insert_UserInfo : Handler<DbService, MsgInsert_UserInfo, ResInsert_UserInfo>
    {
        public Insert_UserInfo(Server server, DbService service) : base(server, service)
        {
        }


        public override MsgType msgType => MsgType._Insert_UserInfo;
        public override async Task<ECode> Handle(IConnection connection, MsgInsert_UserInfo msg, ResInsert_UserInfo res)
        {
            this.service.logger.InfoFormat("{0}, userId: {1}", this.msgType, msg.userInfo.userId);

            await this.service.collection_user_info.Insert(msg.userInfo);
            return ECode.Success;
        }
    }
}