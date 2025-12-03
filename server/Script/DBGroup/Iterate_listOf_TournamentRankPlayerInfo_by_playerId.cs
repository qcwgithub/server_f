using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Iterate_listOf_TournamentRankPlayerInfo_by_playerId : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Iterate_listOf_TournamentRankPlayerInfo_by_playerId;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgIterate_listOf_TournamentRankPlayerInfo_by_playerId>(_msg);
            // this.service.logger.InfoFormat("{0} start_playerId: {1}, end_playerId: {2}", this.msgType, msg.start_playerId, msg.end_playerId);

            var result = await this.service.collection_tournament_rank_player_info.Iterate_listOf_TournamentRankPlayerInfo_by_playerId(msg.start_playerId, msg.end_playerId);

            var res = new ResIterate_listOf_TournamentRankPlayerInfo_by_playerId();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
