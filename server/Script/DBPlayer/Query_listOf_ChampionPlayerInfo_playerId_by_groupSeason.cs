using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_listOf_ChampionPlayerInfo_playerId_by_groupSeason : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Query_listOf_ChampionPlayerInfo_playerId_by_groupSeason;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_listOf_ChampionPlayerInfo_playerId_by_groupSeason>(_msg);
            // this.service.logger.InfoFormat("{0} groupSeason: {1}", this.msgType, msg.groupSeason);

            var result = await this.service.collection_champion_player_info.Query_listOf_ChampionPlayerInfo_playerId_by_groupSeason(msg.groupSeason);

            var res = new ResQuery_listOf_ChampionPlayerInfo_playerId_by_groupSeason();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
