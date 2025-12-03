using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_TournamentPlayerInfo : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Save_TournamentPlayerInfo;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_TournamentPlayerInfo>(_msg);
            this.service.logger.InfoFormat("{0} playerId {1}", this.msgType, msg.info.playerId);

            ECode e = await this.service.collection_tournament_player_info.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_TournamentPlayerInfo();
            return new MyResponse(ECode.Success, res);
        }
    }
}
