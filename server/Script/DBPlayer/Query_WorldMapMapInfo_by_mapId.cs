using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_WorldMapMapInfo_by_mapId : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Query_WorldMapMapInfo_by_mapId;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_WorldMapMapInfo_by_mapId>(_msg);
            // this.service.logger.InfoFormat("{0} mapId: {1}", this.msgType, msg.mapId);

            var result = await this.service.collection_worldmap_map_info.Query_WorldMapMapInfo_by_mapId(msg.mapId);

            var res = new ResQuery_WorldMapMapInfo_by_mapId();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
