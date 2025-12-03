using Data;
using System.Threading.Tasks;

using ApexPlayerBattleSide = Data.PlayerBattleSide;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_ApexPlayerBattleSide_by_playerId : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Query_ApexPlayerBattleSide_by_playerId;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_ApexPlayerBattleSide_by_playerId>(_msg);
            // this.service.logger.InfoFormat("{0} playerId: {1}", this.msgType, msg.playerId);

            var result = await this.service.collection_apex_player_battle_side.Query_ApexPlayerBattleSide_by_playerId(msg.playerId);

            var res = new ResQuery_ApexPlayerBattleSide_by_playerId();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
