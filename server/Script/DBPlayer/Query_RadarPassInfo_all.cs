using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_RadarPassInfo_all : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Query_RadarPassInfo_all;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_RadarPassInfo_all>(_msg);
            // this.service.logger.InfoFormat("{0}", this.msgType);

            var result = await this.service.collection_radar_pass_info.Query_RadarPassInfo_all();

            var res = new ResQuery_RadarPassInfo_all();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
