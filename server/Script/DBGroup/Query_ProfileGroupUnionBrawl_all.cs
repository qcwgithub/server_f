using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_ProfileGroupUnionBrawl_all : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Query_ProfileGroupUnionBrawl_all;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_ProfileGroupUnionBrawl_all>(_msg);
            // this.service.logger.InfoFormat("{0}", this.msgType);

            var result = await this.service.collection_profile_group_union_brawl.Query_ProfileGroupUnionBrawl_all();

            var res = new ResQuery_ProfileGroupUnionBrawl_all();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
