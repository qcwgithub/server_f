using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_ProfileGroupUnionCompetition : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Save_ProfileGroupUnionCompetition;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_ProfileGroupUnionCompetition>(_msg);
            this.service.logger.InfoFormat("{0}", this.msgType);

            ECode e = await this.service.collection_profile_group_union_competition.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_ProfileGroupUnionCompetition();
            return new MyResponse(ECode.Success, res);
        }
    }
}
