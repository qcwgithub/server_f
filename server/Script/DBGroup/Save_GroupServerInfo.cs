using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_GroupServerInfo : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Save_GroupServerInfo;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_GroupServerInfo>(_msg);
            this.service.logger.InfoFormat("{0}", this.msgType);

            ECode e = await this.service.collection_group_server_info.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_GroupServerInfo();
            return new MyResponse(ECode.Success, res);
        }
    }
}
