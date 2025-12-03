using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_AccountInfo_by_channelUserId : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Query_AccountInfo_by_channelUserId;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_AccountInfo_by_channelUserId>(_msg);
            // this.service.logger.InfoFormat("{0} channelUserId: {1}", this.msgType, msg.channelUserId);

            var result = await this.service.collection_account_info.Query_AccountInfo_by_channelUserId(msg.channelUserId);

            var res = new ResQuery_AccountInfo_by_channelUserId();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
