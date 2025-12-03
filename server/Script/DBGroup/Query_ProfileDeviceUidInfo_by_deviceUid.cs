using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_ProfileDeviceUidInfo_by_deviceUid : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Query_ProfileDeviceUidInfo_by_deviceUid;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_ProfileDeviceUidInfo_by_deviceUid>(_msg);
            // this.service.logger.InfoFormat("{0} deviceUid: {1}", this.msgType, msg.deviceUid);

            var result = await this.service.collection_device_uid_info.Query_ProfileDeviceUidInfo_by_deviceUid(msg.deviceUid);

            var res = new ResQuery_ProfileDeviceUidInfo_by_deviceUid();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
