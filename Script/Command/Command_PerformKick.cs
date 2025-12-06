using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;


namespace Script
{
    public class Monitor_PerformKick : Handler<CommandService, MsgCommon>
    {
        public override MsgType msgType => MsgType._Command_PerformKick;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, MsgCommon msg)
        {
            int serviceId = (int)msg.GetLong("serviceId");
            long userId = msg.GetLong("userId");

            this.service.logger.InfoFormat("{0} {1} {2}", this.msgType, serviceId, userId);

            var msgKick = new MsgServerKick();
            msgKick.userId = userId;
            msgKick.logoutSdk = true;

            return await this.service.connectToSameServerType.SendToServiceAsync(serviceId, MsgType._PS_ServerKick, msgKick);
        }
    }
}