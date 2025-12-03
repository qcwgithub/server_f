using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_UnionInfo_maxOf_unionId_by_serverId : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Query_UnionInfo_maxOf_unionId_by_serverId;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_UnionInfo_maxOf_unionId_by_serverId>(_msg);
            // this.service.logger.InfoFormat("{0} serverId: {1}", this.msgType, msg.serverId);

            var result = await this.service.collection_union_info.Query_UnionInfo_maxOf_unionId_by_serverId(msg.serverId);

            var res = new ResQuery_UnionInfo_maxOf_unionId_by_serverId();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
