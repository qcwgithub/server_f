using Data;
using System.Threading.Tasks;

using ExpeditionPartyTeamInfo = Data.TeamInfo;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_ExpeditionPartyTeamInfo_by_teamId : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Query_ExpeditionPartyTeamInfo_by_teamId;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_ExpeditionPartyTeamInfo_by_teamId>(_msg);
            // this.service.logger.InfoFormat("{0} teamId: {1}", this.msgType, msg.teamId);

            var result = await this.service.collection_expedition_party_team_info.Query_ExpeditionPartyTeamInfo_by_teamId(msg.teamId);

            var res = new ResQuery_ExpeditionPartyTeamInfo_by_teamId();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
