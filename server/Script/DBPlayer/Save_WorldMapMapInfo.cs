using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_WorldMapMapInfo : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Save_WorldMapMapInfo;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_WorldMapMapInfo>(_msg);
            this.service.logger.InfoFormat("{0} mapId {1}", this.msgType, msg.info.mapId);

            ECode e = await this.service.collection_worldmap_map_info.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_WorldMapMapInfo();
            return new MyResponse(ECode.Success, res);
        }
    }
}
