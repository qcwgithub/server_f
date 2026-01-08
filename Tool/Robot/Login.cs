using Data;

namespace Tool
{
    public partial class Robot
    {
        public async Task<(ECode, ResLogin?)> Login()
        {
            var msgLogin = new MsgLogin();
            msgLogin.version = string.Empty;
            msgLogin.platform = "Windows";
            msgLogin.channel = MyChannels.uuid;
            msgLogin.channelUserId = this.channelUserId.ToString();

            Console.WriteLine($"Login channelUserId {msgLogin.channelUserId}");

            var r = await this.connection.Request(MsgType.Login, msgLogin);
            Console.WriteLine($"Login result {r.e}");
            if (r.e != ECode.Success)
            {
                return (r.e, null);
            }

            var resLogin = r.CastRes<ResLogin>();
            Console.WriteLine($"isNewUser? {resLogin.isNewUser} userId {resLogin.userInfo.userId} kickOther? {resLogin.kickOther}");
            return (ECode.Success, resLogin);
        }
    }
}