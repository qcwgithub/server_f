using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;


namespace Script
{
    public class Command_PerformSetPlayerGmFlag : Handler<CommandService>
    {
        public override MsgType msgType => MsgType._Command_PerformSetPlayerGmFlag;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgCommon>(_msg);
            long startId = msg.GetLongId("startId");
            long endId = msg.GetLongId("endId");
            int serviceId = (int)msg.GetLong("serviceId");

            var msgSet = new MsgSetGmFlag();
            msgSet.startPlayerId = startId;
            msgSet.endPlayerId = endId;

            var r = await this.service.connectToSameServerType.SendToServiceAsync(serviceId, MsgType._SetGmFlag, msgSet);
            var res = r.CastRes<ResSetGmFlag>();
            if (res.listPlayer != null)
            {
                foreach (var item in res.listPlayer)
                {
                    this.service.logger.InfoFormat("{0} success set GM!", item);
                }
            }
            return r;
        }
    }
}