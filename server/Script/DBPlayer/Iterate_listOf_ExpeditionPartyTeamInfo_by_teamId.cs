using Data;
using System.Threading.Tasks;

using ExpeditionPartyTeamInfo = Data.TeamInfo;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Iterate_listOf_ExpeditionPartyTeamInfo_by_teamId : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Iterate_listOf_ExpeditionPartyTeamInfo_by_teamId;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgIterate_listOf_ExpeditionPartyTeamInfo_by_teamId>(_msg);
            // this.service.logger.InfoFormat("{0} start_teamId: {1}, end_teamId: {2}", this.msgType, msg.start_teamId, msg.end_teamId);

            var result = await this.service.collection_expedition_party_team_info.Iterate_listOf_ExpeditionPartyTeamInfo_by_teamId(msg.start_teamId, msg.end_teamId);

            var res = new ResIterate_listOf_ExpeditionPartyTeamInfo_by_teamId();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
