using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_ProfileGroupUnionClash_all : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Query_ProfileGroupUnionClash_all;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_ProfileGroupUnionClash_all>(_msg);
            // this.service.logger.InfoFormat("{0}", this.msgType);

            var result = await this.service.collection_profile_group_union_clash.Query_ProfileGroupUnionClash_all();

            var res = new ResQuery_ProfileGroupUnionClash_all();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
