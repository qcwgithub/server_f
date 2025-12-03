using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Iterate_listOf_WorldMapMapInfo_by_playerOrUnionId : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Iterate_listOf_WorldMapMapInfo_by_playerOrUnionId;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgIterate_listOf_WorldMapMapInfo_by_playerOrUnionId>(_msg);
            // this.service.logger.InfoFormat("{0} start_playerOrUnionId: {1}, end_playerOrUnionId: {2}", this.msgType, msg.start_playerOrUnionId, msg.end_playerOrUnionId);

            var result = await this.service.collection_worldmap_map_info.Iterate_listOf_WorldMapMapInfo_by_playerOrUnionId(msg.start_playerOrUnionId, msg.end_playerOrUnionId);

            var res = new ResIterate_listOf_WorldMapMapInfo_by_playerOrUnionId();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
