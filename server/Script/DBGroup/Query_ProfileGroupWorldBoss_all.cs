using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_ProfileGroupWorldBoss_all : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Query_ProfileGroupWorldBoss_all;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_ProfileGroupWorldBoss_all>(_msg);
            // this.service.logger.InfoFormat("{0}", this.msgType);

            var result = await this.service.collection_profile_group_world_boss.Query_ProfileGroupWorldBoss_all();

            var res = new ResQuery_ProfileGroupWorldBoss_all();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
