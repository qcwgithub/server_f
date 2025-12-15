using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;


namespace Script
{
    public class Command_PerformSaveUserInfoToFile : Handler<CommandService, MsgCommon, ResCommon>
    {
        public Command_PerformSaveUserInfoToFile(Server server, CommandService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Command_PerformSaveUserInfoToFile;

        public override async Task<ECode> Handle(IConnection connection, MsgCommon msg, ResCommon res)
        {
            long userId = msg.GetLong("userId");
            int serviceId = (int)msg.GetLong("serviceId");

            var msg2 = new MsgSaveUserInfoToFile();
            msg2.userId = userId;
            var r = await this.service.connectToSameServerType.Request<MsgSaveUserInfoToFile, ResSaveUserInfoToFile>(serviceId, MsgType._SaveUserInfoToFile, msg2);
            if (r.e == ECode.Success)
            {
                this.service.logger.Info("save user info to file ok, file name: " + r.res.fileName);
            }
            return r.e;
        }
    }
}