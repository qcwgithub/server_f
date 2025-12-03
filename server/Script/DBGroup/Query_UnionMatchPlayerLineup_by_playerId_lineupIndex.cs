using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_UnionMatchPlayerLineup_by_playerId_lineupIndex : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Query_UnionMatchPlayerLineup_by_playerId_lineupIndex;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_UnionMatchPlayerLineup_by_playerId_lineupIndex>(_msg);
            // this.service.logger.InfoFormat("{0} playerId: {1}, lineupIndex: {2}", this.msgType, msg.playerId, msg.lineupIndex);

            var result = await this.service.collection_union_match_player_lineup.Query_UnionMatchPlayerLineup_by_playerId_lineupIndex(msg.playerId, msg.lineupIndex);

            var res = new ResQuery_UnionMatchPlayerLineup_by_playerId_lineupIndex();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
