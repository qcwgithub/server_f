using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class Command_PerformShutdown : Handler<CommandService>
    {
        public override MsgType msgType => MsgType._Command_PerformShutdown;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgCommon>(_msg);
            int serviceId = (int)msg.GetLong("serviceId");
            int force = (int)msg.GetLong("force");

            var msgShutdown = new MsgShutdown();
            msgShutdown.force = force == 1;
            return await this.service.connectToSameServerType.SendToServiceAsync(serviceId, MsgType._Shutdown, msgShutdown);
        }
    }
}