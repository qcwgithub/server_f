using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;


namespace Script
{
    public class Monitor_PerformKick : Handler<CommandService, MsgCommon, ResCommon>
    {
        public Monitor_PerformKick(Server server, CommandService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Command_PerformKick;

        public override async Task<ECode> Handle(IConnection connection, MsgCommon msg, ResCommon res)
        {
            int serviceId = (int)msg.GetLong("serviceId");
            long userId = msg.GetLong("userId");

            this.service.logger.InfoFormat("{0} {1} {2}", this.msgType, serviceId, userId);

            var msgKick = new MsgGatewayServerKick();
            msgKick.userId = userId;
            msgKick.logoutSdk = true;

            var r = await this.service.connectToSameServerType.Request<MsgGatewayServerKick, ResGatewayServerKick>(serviceId, MsgType._Gateway_ServerKick, msgKick);
            return r.e;
        }
    }
}