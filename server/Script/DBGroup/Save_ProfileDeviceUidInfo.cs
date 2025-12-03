using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_ProfileDeviceUidInfo : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Save_ProfileDeviceUidInfo;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_ProfileDeviceUidInfo>(_msg);
            this.service.logger.InfoFormat("{0} deviceUid {1}", this.msgType, msg.info.deviceUid);

            ECode e = await this.service.collection_device_uid_info.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_ProfileDeviceUidInfo();
            return new MyResponse(ECode.Success, res);
        }
    }
}
