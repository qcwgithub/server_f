using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_ExpeditionInfo_all : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Query_ExpeditionInfo_all;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_ExpeditionInfo_all>(_msg);
            // this.service.logger.InfoFormat("{0}", this.msgType);

            var result = await this.service.collection_expedition_info.Query_ExpeditionInfo_all();

            var res = new ResQuery_ExpeditionInfo_all();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
