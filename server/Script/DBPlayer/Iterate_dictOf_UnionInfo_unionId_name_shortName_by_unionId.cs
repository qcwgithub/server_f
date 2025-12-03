using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Iterate_dictOf_UnionInfo_unionId_name_shortName_by_unionId : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Iterate_dictOf_UnionInfo_unionId_name_shortName_by_unionId;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgIterate_dictOf_UnionInfo_unionId_name_shortName_by_unionId>(_msg);
            // this.service.logger.InfoFormat("{0} start_unionId: {1}, end_unionId: {2}", this.msgType, msg.start_unionId, msg.end_unionId);

            var result = await this.service.collection_union_info.Iterate_dictOf_UnionInfo_unionId_name_shortName_by_unionId(msg.start_unionId, msg.end_unionId);

            var res = new ResIterate_dictOf_UnionInfo_unionId_name_shortName_by_unionId();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
