using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_AccountInfo_byElementOf_playerIds : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Query_AccountInfo_byElementOf_playerIds;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_AccountInfo_byElementOf_playerIds>(_msg);
            // this.service.logger.InfoFormat("{0} ele_playerIds: {1}", this.msgType, msg.ele_playerIds);

            var result = await this.service.collection_account_info.Query_AccountInfo_byElementOf_playerIds(msg.ele_playerIds);

            var res = new ResQuery_AccountInfo_byElementOf_playerIds();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
