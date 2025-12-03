using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_PlayerUnionInfo_by_playerId : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Query_PlayerUnionInfo_by_playerId;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_PlayerUnionInfo_by_playerId>(_msg);
            // this.service.logger.InfoFormat("{0} playerId: {1}", this.msgType, msg.playerId);

            var result = await this.service.collection_player_union_info.Query_PlayerUnionInfo_by_playerId(msg.playerId);

            var res = new ResQuery_PlayerUnionInfo_by_playerId();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
