using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_ArenaRobotInfo_all : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Query_ArenaRobotInfo_all;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_ArenaRobotInfo_all>(_msg);
            // this.service.logger.InfoFormat("{0}", this.msgType);

            var result = await this.service.collection_arena_robot_info.Query_ArenaRobotInfo_all();

            var res = new ResQuery_ArenaRobotInfo_all();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
