using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_AccountInfo_by_channel_channelUserId : Handler<DbService, MsgQuery_AccountInfo_by_channel_channelUserId, ResQuery_AccountInfo_by_channel_channelUserId>
    {
        public override MsgType msgType => MsgType._Query_AccountInfo_by_channel_channelUserId;

        public Query_AccountInfo_by_channel_channelUserId(Server server, DbService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgQuery_AccountInfo_by_channel_channelUserId msg, ResQuery_AccountInfo_by_channel_channelUserId res)
        {
            // this.service.logger.InfoFormat("{0} channel: {1}, channelUserId: {2}", this.msgType, msg.channel, msg.channelUserId);

            var result = await this.service.collection_account_info.Query_AccountInfo_by_channel_channelUserId(msg.channel, msg.channelUserId);

            res.result = result;
            return ECode.Success;
        }
    }
}
