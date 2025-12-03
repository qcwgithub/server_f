using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_AccountInfo_by_channelUserId_2 : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Query_AccountInfo_by_channelUserId_2;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_AccountInfo_by_channelUserId_2>(_msg);
            // this.service.logger.InfoFormat("{0} channelUserId_2: {1}", this.msgType, msg.channelUserId_2);

            var result = await this.service.collection_account_info.Query_AccountInfo_by_channelUserId_2(msg.channelUserId_2);

            var res = new ResQuery_AccountInfo_by_channelUserId_2();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
