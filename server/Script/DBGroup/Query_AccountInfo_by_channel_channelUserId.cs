using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_AccountInfo_by_channel_channelUserId : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Query_AccountInfo_by_channel_channelUserId;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_AccountInfo_by_channel_channelUserId>(_msg);
            // this.service.logger.InfoFormat("{0} channel: {1}, channelUserId: {2}", this.msgType, msg.channel, msg.channelUserId);

            var result = await this.service.collection_account_info.Query_AccountInfo_by_channel_channelUserId(msg.channel, msg.channelUserId);

            var res = new ResQuery_AccountInfo_by_channel_channelUserId();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
