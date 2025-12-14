using Data;

namespace Script
{
    public class Db_InsertUserInfo : Handler<DbService, MsgInsertUserInfo, ResInsertUserInfo>
    {
        public Db_InsertUserInfo(Server server, DbService service) : base(server, service)
        {
        }


        public override MsgType msgType => MsgType._Db_InsertUserInfo;
        public override async Task<ECode> Handle(IConnection connection, MsgInsertUserInfo msg, ResInsertUserInfo res)
        {
            this.service.logger.InfoFormat("{0}, userId: {1}", this.msgType, msg.userInfo.userId);

            await this.service.collection_user_info.Insert(msg.userInfo);
            return ECode.Success;
        }
    }
}