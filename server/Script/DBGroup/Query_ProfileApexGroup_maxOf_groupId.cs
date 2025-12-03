using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_ProfileApexGroup_maxOf_groupId : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Query_ProfileApexGroup_maxOf_groupId;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_ProfileApexGroup_maxOf_groupId>(_msg);
            // this.service.logger.InfoFormat("{0}", this.msgType);

            var result = await this.service.collection_profile_apex_group.Query_ProfileApexGroup_maxOf_groupId();

            var res = new ResQuery_ProfileApexGroup_maxOf_groupId();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
