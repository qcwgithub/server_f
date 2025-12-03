using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_ProfileGroupUnionTreasure_all : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Query_ProfileGroupUnionTreasure_all;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_ProfileGroupUnionTreasure_all>(_msg);
            // this.service.logger.InfoFormat("{0}", this.msgType);

            var result = await this.service.collection_profile_group_union_treasure.Query_ProfileGroupUnionTreasure_all();

            var res = new ResQuery_ProfileGroupUnionTreasure_all();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
