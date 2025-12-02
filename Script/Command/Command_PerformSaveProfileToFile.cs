using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;


namespace Script
{
    public class Command_PerformSaveProfileToFile : Handler<CommandService>
    {
        public override MsgType msgType => MsgType._Command_PerformSaveProfileToFile;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgCommon>(_msg);
            long playerId = msg.GetLongId("playerId");
            int serviceId = (int)msg.GetLong("serviceId");

            var msg2 = new MsgSaveProfileToFile();
            msg2.playerId = playerId;
            MyResponse r = await this.service.connectToSameServerType.SendToServiceAsync(serviceId, MsgType._SaveProfileToFile, msg2);
            if (r.err == ECode.Success)
            {
                var res = r.CastRes<ResSaveProfileToFile>();
                this.service.logger.Info("save profile to file ok, file name: " + res.fileName);
            }
            return r;
        }
    }
}