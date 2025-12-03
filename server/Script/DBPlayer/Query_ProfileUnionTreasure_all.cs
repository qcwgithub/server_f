using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_ProfileUnionTreasure_all : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Query_ProfileUnionTreasure_all;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_ProfileUnionTreasure_all>(_msg);
            // this.service.logger.InfoFormat("{0}", this.msgType);

            var result = await this.service.collection_profile_union_treasure.Query_ProfileUnionTreasure_all();

            var res = new ResQuery_ProfileUnionTreasure_all();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
