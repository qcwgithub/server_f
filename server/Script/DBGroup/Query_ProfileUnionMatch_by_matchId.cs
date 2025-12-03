using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_ProfileUnionMatch_by_matchId : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Query_ProfileUnionMatch_by_matchId;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_ProfileUnionMatch_by_matchId>(_msg);
            // this.service.logger.InfoFormat("{0} matchId: {1}", this.msgType, msg.matchId);

            var result = await this.service.collection_profile_union_match.Query_ProfileUnionMatch_by_matchId(msg.matchId);

            var res = new ResQuery_ProfileUnionMatch_by_matchId();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
