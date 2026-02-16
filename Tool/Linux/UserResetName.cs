using Data;

namespace Tool
{
    public partial class LinuxProgram
    {
        async Task UserResetName()
        {
            string s = AskHelp.AskInput("user id?").OnAnswer();
            if (!long.TryParse(s, out long userId))
            {
                return;
            }

            var serviceConfig = this.allServiceConfigs.Find(tai => tai.serviceType == ServiceType.UserManager);
            var r = await this.Connect_Request_Close(serviceConfig, MsgType._UserManager_GetUserLocation, new MsgUserManagerGetUserLocation
            {
                userId = userId,
                addWhenNotExist = true
            });
            if (r.e != ECode.Success)
            {
                return;
            }

            var res = r.CastRes<ResUserManagerGetUserLocation>();
            MyDebug.Assert(res.location.IsValid());

            serviceConfig = this.allServiceConfigs.Find(tai => tai.serviceId == res.location.serviceId);

            await this.Connect_Request_Close(serviceConfig, MsgType._User_ResetName, new MsgResetName
            {
                userId = userId
            });
        }
    }
}