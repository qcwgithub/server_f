using Data;

namespace Tool
{
    public partial class Robot
    {
        public async Task<(ECode, ResLogin?)> Login()
        {
            var msg = new MsgLogin();
            msg.version = string.Empty;
            msg.platform = "Windows";
            msg.channel = MyChannels.uuid;
            msg.channelUserId = this.channelUserId.ToString();

            this.Log($"Login");

            var r = await this.connection.Request(MsgType.Login, msg);
            this.Log($"Login result {r.e}");
            if (r.e != ECode.Success)
            {
                return (r.e, null);
            }

            var resLogin = r.CastRes<ResLogin>();
            this.Log($"isNewUser? {resLogin.isNewUser} userId {resLogin.userInfo.userId} kickOther? {resLogin.kickOther}");
            return (ECode.Success, resLogin);
        }
    }
}