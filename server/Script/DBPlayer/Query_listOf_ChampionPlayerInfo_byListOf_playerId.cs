using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_listOf_ChampionPlayerInfo_byListOf_playerId : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Query_listOf_ChampionPlayerInfo_byListOf_playerId;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_listOf_ChampionPlayerInfo_byListOf_playerId>(_msg);
            // this.service.logger.InfoFormat("{0} playerIdList: {1}", this.msgType, msg.playerIdList);

            var result = await this.service.collection_champion_player_info.Query_listOf_ChampionPlayerInfo_byListOf_playerId(msg.playerIdList);

            var res = new ResQuery_listOf_ChampionPlayerInfo_byListOf_playerId();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
