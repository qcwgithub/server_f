using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_ApexRobotInfo : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Save_ApexRobotInfo;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_ApexRobotInfo>(_msg);
            this.service.logger.InfoFormat("{0}", this.msgType);

            ECode e = await this.service.collection_apex_robot_info.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_ApexRobotInfo();
            return new MyResponse(ECode.Success, res);
        }
    }
}
