using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_ProfileTournamentGroup : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Save_ProfileTournamentGroup;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_ProfileTournamentGroup>(_msg);
            this.service.logger.InfoFormat("{0} groupId {1}", this.msgType, msg.info.groupId);

            ECode e = await this.service.collection_profile_tournament_group.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_ProfileTournamentGroup();
            return new MyResponse(ECode.Success, res);
        }
    }
}
