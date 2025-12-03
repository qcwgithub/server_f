using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_ProfileGroupGodRoulette_all : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Query_ProfileGroupGodRoulette_all;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_ProfileGroupGodRoulette_all>(_msg);
            // this.service.logger.InfoFormat("{0}", this.msgType);

            var result = await this.service.collection_group_god_roulette.Query_ProfileGroupGodRoulette_all();

            var res = new ResQuery_ProfileGroupGodRoulette_all();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
