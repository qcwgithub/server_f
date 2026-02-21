using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    [AutoRegister]
    public sealed class Query_UserBriefInfo_by_userId : Handler<DbService, MsgQuery_UserBriefInfo_by_userId, ResQuery_UserBriefInfo_by_userId>
    {
        public override MsgType msgType => MsgType._Query_UserBriefInfo_by_userId;

        public Query_UserBriefInfo_by_userId(Server server, DbService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgQuery_UserBriefInfo_by_userId msg, ResQuery_UserBriefInfo_by_userId res)
        {
            // this.service.logger.InfoFormat("{0} userId: {1}", this.msgType, msg.userId);

            var result = await this.service.collection_user_brief_info.Query_UserBriefInfo_by_userId(msg.userId);

            res.result = result;
            return ECode.Success;
        }
    }
}
