using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_UserInfo_maxOf_userId : Handler<DbService, MsgQuery_UserInfo_maxOf_userId, ResQuery_UserInfo_maxOf_userId>
    {
        public override MsgType msgType => MsgType._Query_UserInfo_maxOf_userId;

        public Query_UserInfo_maxOf_userId(Server server, DbService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MsgContext context, MsgQuery_UserInfo_maxOf_userId msg, ResQuery_UserInfo_maxOf_userId res)
        {
            // this.service.logger.InfoFormat("{0}", this.msgType);

            var result = await this.service.collection_user_info.Query_UserInfo_maxOf_userId();

            res.result = result;
            return ECode.Success;
        }
    }
}
