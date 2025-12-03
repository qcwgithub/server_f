using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_UnionMatchNpcLineup_by_npcId : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Query_UnionMatchNpcLineup_by_npcId;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_UnionMatchNpcLineup_by_npcId>(_msg);
            // this.service.logger.InfoFormat("{0} npcId: {1}", this.msgType, msg.npcId);

            var result = await this.service.collection_union_match_npc_lineup.Query_UnionMatchNpcLineup_by_npcId(msg.npcId);

            var res = new ResQuery_UnionMatchNpcLineup_by_npcId();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
