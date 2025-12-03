using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_UnionCUnionInfo_by_unionId : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Query_UnionCUnionInfo_by_unionId;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_UnionCUnionInfo_by_unionId>(_msg);
            // this.service.logger.InfoFormat("{0} unionId: {1}", this.msgType, msg.unionId);

            var result = await this.service.collection_unionc_union_info.Query_UnionCUnionInfo_by_unionId(msg.unionId);

            var res = new ResQuery_UnionCUnionInfo_by_unionId();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
