using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_listOf_AccountInfo_byElementOf_userIds : Handler<DbService, MsgQuery_listOf_AccountInfo_byElementOf_userIds, ResQuery_listOf_AccountInfo_byElementOf_userIds>
    {
        public override MsgType msgType => MsgType._Query_listOf_AccountInfo_byElementOf_userIds;

        public Query_listOf_AccountInfo_byElementOf_userIds(Server server, DbService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgQuery_listOf_AccountInfo_byElementOf_userIds msg, ResQuery_listOf_AccountInfo_byElementOf_userIds res)
        {
            // this.service.logger.InfoFormat("{0} ele_userIds: {1}", this.msgType, msg.ele_userIds);

            var result = await this.service.collection_account_info.Query_listOf_AccountInfo_byElementOf_userIds(msg.ele_userIds);

            res.result = result;
            return ECode.Success;
        }
    }
}
