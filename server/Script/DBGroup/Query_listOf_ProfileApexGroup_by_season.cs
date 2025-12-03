using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_listOf_ProfileApexGroup_by_season : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Query_listOf_ProfileApexGroup_by_season;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_listOf_ProfileApexGroup_by_season>(_msg);
            // this.service.logger.InfoFormat("{0} season: {1}", this.msgType, msg.season);

            var result = await this.service.collection_profile_apex_group.Query_listOf_ProfileApexGroup_by_season(msg.season);

            var res = new ResQuery_listOf_ProfileApexGroup_by_season();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
