using Data;

namespace Script
{
    public partial class CommandService
    {
        public async Task<ECode> PerformSaveUserInfoToFile(MsgCommon msg)
        {
            long userId = msg.GetLong("userId");
            int serviceId = (int)msg.GetLong("serviceId");

            var msg2 = new MsgSaveUserInfoToFile();
            msg2.userId = userId;
            var r = await this.commandConnectToOtherService.Request(serviceId, MsgType._User_SaveUserInfoToFile, msg2);
            if (r.e == ECode.Success)
            {
                this.logger.Info("save user info to file ok, file name: " + r.CastRes<ResSaveUserInfoToFile>().fileName);
            }
            return r.e;
        }
    }
}