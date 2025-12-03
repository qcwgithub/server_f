using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_ExpeditionPartyPlayerInfo_by_playerId : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Query_ExpeditionPartyPlayerInfo_by_playerId;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_ExpeditionPartyPlayerInfo_by_playerId>(_msg);
            // this.service.logger.InfoFormat("{0} playerId: {1}", this.msgType, msg.playerId);

            var result = await this.service.collection_expedition_party_player_info.Query_ExpeditionPartyPlayerInfo_by_playerId(msg.playerId);

            var res = new ResQuery_ExpeditionPartyPlayerInfo_by_playerId();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
