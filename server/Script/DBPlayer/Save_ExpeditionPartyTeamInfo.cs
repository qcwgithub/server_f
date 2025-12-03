using Data;
using System.Threading.Tasks;

using ExpeditionPartyTeamInfo = Data.TeamInfo;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_ExpeditionPartyTeamInfo : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Save_ExpeditionPartyTeamInfo;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_ExpeditionPartyTeamInfo>(_msg);
            this.service.logger.InfoFormat("{0} teamId {1}", this.msgType, msg.info.teamId);

            ECode e = await this.service.collection_expedition_party_team_info.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_ExpeditionPartyTeamInfo();
            return new MyResponse(ECode.Success, res);
        }
    }
}
