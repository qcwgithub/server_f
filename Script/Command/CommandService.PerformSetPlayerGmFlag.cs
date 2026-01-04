using Data;

namespace Script
{
    public partial class CommandService
    {
        public async Task<ECode> PerformSetPlayerGmFlag(MsgCommon msg)
        {
            long startId = msg.GetLong("startId");
            long endId = msg.GetLong("endId");
            int serviceId = (int)msg.GetLong("serviceId");

            var msgSet = new MsgSetGmFlag();
            msgSet.startUserId = startId;
            msgSet.endUserId = endId;

            var r = await this.commandConnectToOtherService.Request(serviceId, MsgType._User_SetGmFlag, msgSet);
            if (r.res != null)
            {
                var resSet = r.CastRes<ResSetGmFlag>();
                if (resSet.listUser != null)
                {
                    foreach (var item in resSet.listUser)
                    {
                        this.logger.InfoFormat("{0} success set GM!", item);
                    }
                }
            }
            return r.e;
        }
    }
}