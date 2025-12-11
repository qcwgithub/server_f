using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;


namespace Script
{
    public class Command_PerformSaveProfileToFile : Handler<CommandService, MsgCommon, ResCommon>
    {
        public Command_PerformSaveProfileToFile(Server server, CommandService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Command_PerformSaveProfileToFile;

        public override async Task<ECode> Handle(IConnection connection, MsgCommon msg, ResCommon res)
        {
            long userId = msg.GetLong("userId");
            int serviceId = (int)msg.GetLong("serviceId");

            var msg2 = new MsgSaveProfileToFile();
            msg2.userId = userId;
            var r = await this.service.connectToSameServerType.RequestToService<MsgSaveProfileToFile, ResSaveProfileToFile>(serviceId, MsgType._SaveProfileToFile, msg2);
            if (r.e == ECode.Success)
            {
                this.service.logger.Info("save profile to file ok, file name: " + r.res.fileName);
            }
            return r.e;
        }
    }
}