using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_ProfileGroupMatchGame_all : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Query_ProfileGroupMatchGame_all;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_ProfileGroupMatchGame_all>(_msg);
            // this.service.logger.InfoFormat("{0}", this.msgType);

            var result = await this.service.collection_profile_group_match_game.Query_ProfileGroupMatchGame_all();

            var res = new ResQuery_ProfileGroupMatchGame_all();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
