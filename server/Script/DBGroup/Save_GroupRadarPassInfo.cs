using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_GroupRadarPassInfo : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Save_GroupRadarPassInfo;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_GroupRadarPassInfo>(_msg);
            this.service.logger.InfoFormat("{0}", this.msgType);

            ECode e = await this.service.collection_group_radar_pass_info.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_GroupRadarPassInfo();
            return new MyResponse(ECode.Success, res);
        }
    }
}
