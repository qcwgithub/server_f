using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_AccountInfo : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Save_AccountInfo;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_AccountInfo>(_msg);
            this.service.logger.InfoFormat("{0} channel {1} channelUserId {2}", this.msgType, msg.info.channel, msg.info.channelUserId);

            ECode e = await this.service.collection_account_info.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_AccountInfo();
            return new MyResponse(ECode.Success, res);
        }
    }
}
