using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_UnionMatchNpcLineup : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Save_UnionMatchNpcLineup;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_UnionMatchNpcLineup>(_msg);
            this.service.logger.InfoFormat("{0} npcId {1}", this.msgType, msg.info.npcId);

            ECode e = await this.service.collection_union_match_npc_lineup.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_UnionMatchNpcLineup();
            return new MyResponse(ECode.Success, res);
        }
    }
}
