using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_UserInfo_by_userId : Handler<DbService, MsgQuery_UserInfo_by_userId, ResQuery_UserInfo_by_userId>
    {
        public override MsgType msgType => MsgType._Query_UserInfo_by_userId;

        public Query_UserInfo_by_userId(Server server, DbService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MsgContext context, MsgQuery_UserInfo_by_userId msg, ResQuery_UserInfo_by_userId res)
        {
            // this.service.logger.InfoFormat("{0} userId: {1}", this.msgType, msg.userId);

            var result = await this.service.collection_user_info.Query_UserInfo_by_userId(msg.userId);

            res.result = result;
            return ECode.Success;
        }
    }
}
