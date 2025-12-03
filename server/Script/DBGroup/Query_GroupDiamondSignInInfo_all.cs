using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_GroupDiamondSignInInfo_all : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Query_GroupDiamondSignInInfo_all;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_GroupDiamondSignInInfo_all>(_msg);
            // this.service.logger.InfoFormat("{0}", this.msgType);

            var result = await this.service.collection_group_diamond_sign_in_info.Query_GroupDiamondSignInInfo_all();

            var res = new ResQuery_GroupDiamondSignInInfo_all();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
