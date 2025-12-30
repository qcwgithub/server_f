using Data;

namespace Script
{
    public class User_UserDisconnectFromGateway : UserHandler<MsgUserDisconnectFromGateway, ResUserDisconnectFromGateway>
    {
        public User_UserDisconnectFromGateway(Server server, UserService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._User_UserDisconnectFromGateway;

        public override async Task<ECode> Handle(MsgContext context, MsgUserDisconnectFromGateway msg, ResUserDisconnectFromGateway res)
        {
            User? user = this.service.sd.GetUser(msg.userId);
            if (user == null)
            {
                return ECode.Success;
            }

            long nowS = TimeUtils.GetTimeS();
            user.offlineTimeS = nowS;

            this.service.ss.SetDestroyTimer(user, UserDestroyUserReason.DestroyTimer_DisconnectFromGateway);
            return ECode.Success;
        }
    }
}