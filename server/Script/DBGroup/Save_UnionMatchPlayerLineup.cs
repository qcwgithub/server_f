using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_UnionMatchPlayerLineup : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Save_UnionMatchPlayerLineup;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_UnionMatchPlayerLineup>(_msg);
            this.service.logger.InfoFormat("{0} playerId {1} lineupIndex {2}", this.msgType, msg.info.playerId, msg.info.lineupIndex);

            ECode e = await this.service.collection_union_match_player_lineup.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_UnionMatchPlayerLineup();
            return new MyResponse(ECode.Success, res);
        }
    }
}
