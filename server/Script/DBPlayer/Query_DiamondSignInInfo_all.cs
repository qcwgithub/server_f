using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_DiamondSignInInfo_all : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Query_DiamondSignInInfo_all;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_DiamondSignInInfo_all>(_msg);
            // this.service.logger.InfoFormat("{0}", this.msgType);

            var result = await this.service.collection_diamond_sign_in_info.Query_DiamondSignInInfo_all();

            var res = new ResQuery_DiamondSignInInfo_all();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
