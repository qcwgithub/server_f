using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_ArenaGroupRobotInfo_by_groupType_groupId : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Query_ArenaGroupRobotInfo_by_groupType_groupId;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_ArenaGroupRobotInfo_by_groupType_groupId>(_msg);
            // this.service.logger.InfoFormat("{0} groupType: {1}, groupId: {2}", this.msgType, msg.groupType, msg.groupId);

            var result = await this.service.collection_arena_group_robot_info.Query_ArenaGroupRobotInfo_by_groupType_groupId(msg.groupType, msg.groupId);

            var res = new ResQuery_ArenaGroupRobotInfo_by_groupType_groupId();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
