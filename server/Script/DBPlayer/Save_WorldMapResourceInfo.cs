using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_WorldMapResourceInfo : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Save_WorldMapResourceInfo;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_WorldMapResourceInfo>(_msg);
            this.service.logger.InfoFormat("{0} mapId {1} resourceId {2}", this.msgType, msg.info.mapId, msg.info.resourceId);

            ECode e = await this.service.collection_worldmap_resource_info.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_WorldMapResourceInfo();
            return new MyResponse(ECode.Success, res);
        }
    }
}
