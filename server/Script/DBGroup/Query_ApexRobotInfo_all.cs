using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_ApexRobotInfo_all : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Query_ApexRobotInfo_all;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_ApexRobotInfo_all>(_msg);
            // this.service.logger.InfoFormat("{0}", this.msgType);

            var result = await this.service.collection_apex_robot_info.Query_ApexRobotInfo_all();

            var res = new ResQuery_ApexRobotInfo_all();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
