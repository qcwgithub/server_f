using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_ProfileUnionMatch : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Save_ProfileUnionMatch;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_ProfileUnionMatch>(_msg);
            this.service.logger.InfoFormat("{0} matchId {1}", this.msgType, msg.info.matchId);

            ECode e = await this.service.collection_profile_union_match.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_ProfileUnionMatch();
            return new MyResponse(ECode.Success, res);
        }
    }
}
