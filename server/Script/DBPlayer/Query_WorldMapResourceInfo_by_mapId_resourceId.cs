using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_WorldMapResourceInfo_by_mapId_resourceId : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Query_WorldMapResourceInfo_by_mapId_resourceId;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_WorldMapResourceInfo_by_mapId_resourceId>(_msg);
            // this.service.logger.InfoFormat("{0} mapId: {1}, resourceId: {2}", this.msgType, msg.mapId, msg.resourceId);

            var result = await this.service.collection_worldmap_resource_info.Query_WorldMapResourceInfo_by_mapId_resourceId(msg.mapId, msg.resourceId);

            var res = new ResQuery_WorldMapResourceInfo_by_mapId_resourceId();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
