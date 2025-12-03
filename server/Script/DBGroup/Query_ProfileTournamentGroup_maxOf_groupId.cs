using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_ProfileTournamentGroup_maxOf_groupId : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Query_ProfileTournamentGroup_maxOf_groupId;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_ProfileTournamentGroup_maxOf_groupId>(_msg);
            // this.service.logger.InfoFormat("{0}", this.msgType);

            var result = await this.service.collection_profile_tournament_group.Query_ProfileTournamentGroup_maxOf_groupId();

            var res = new ResQuery_ProfileTournamentGroup_maxOf_groupId();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
