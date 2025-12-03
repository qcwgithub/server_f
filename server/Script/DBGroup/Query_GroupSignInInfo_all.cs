using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_GroupSignInInfo_all : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Query_GroupSignInInfo_all;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_GroupSignInInfo_all>(_msg);
            // this.service.logger.InfoFormat("{0}", this.msgType);

            var result = await this.service.collection_group_sign_in_info.Query_GroupSignInInfo_all();

            var res = new ResQuery_GroupSignInInfo_all();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
