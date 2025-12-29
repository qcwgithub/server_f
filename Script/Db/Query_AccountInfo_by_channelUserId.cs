using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_AccountInfo_by_channelUserId : Handler<DbService, MsgQuery_AccountInfo_by_channelUserId, ResQuery_AccountInfo_by_channelUserId>
    {
        public override MsgType msgType => MsgType._Query_AccountInfo_by_channelUserId;

        public Query_AccountInfo_by_channelUserId(Server server, DbService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MsgContext context, MsgQuery_AccountInfo_by_channelUserId msg, ResQuery_AccountInfo_by_channelUserId res)
        {
            // this.service.logger.InfoFormat("{0} channelUserId: {1}", this.msgType, msg.channelUserId);

            var result = await this.service.collection_account_info.Query_AccountInfo_by_channelUserId(msg.channelUserId);

            res.result = result;
            return ECode.Success;
        }
    }
}
