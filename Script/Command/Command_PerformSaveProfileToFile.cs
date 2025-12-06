using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;


namespace Script
{
    public class Command_PerformSaveProfileToFile : Handler<CommandService, MsgCommon>
    {
        public override MsgType msgType => MsgType._Command_PerformSaveProfileToFile;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, MsgCommon msg)
        {
            long userId = msg.GetLong("userId");
            int serviceId = (int)msg.GetLong("serviceId");

            var msg2 = new MsgSaveProfileToFile();
            msg2.userId = userId;
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