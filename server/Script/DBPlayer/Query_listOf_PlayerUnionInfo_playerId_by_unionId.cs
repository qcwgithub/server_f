using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_listOf_PlayerUnionInfo_playerId_by_unionId : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Query_listOf_PlayerUnionInfo_playerId_by_unionId;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_listOf_PlayerUnionInfo_playerId_by_unionId>(_msg);
            // this.service.logger.InfoFormat("{0} unionId: {1}", this.msgType, msg.unionId);

            var result = await this.service.collection_player_union_info.Query_listOf_PlayerUnionInfo_playerId_by_unionId(msg.unionId);

            var res = new ResQuery_listOf_PlayerUnionInfo_playerId_by_unionId();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
