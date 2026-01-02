using Data;

namespace Script
{
    public class User_UserDisconnectFromGateway : UserHandler<MsgUserDisconnectFromGateway, ResUserDisconnectFromGateway>
    {
        public User_UserDisconnectFromGateway(Server server, UserService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._User_UserDisconnectFromGateway;

        public override async Task<ECode> Handle(MessageContext context, MsgUserDisconnectFromGateway msg, ResUserDisconnectFromGateway res)
        {
            User? user = await this.service.LockUser(msg.userId, context);
            if (user == null)
            {
                return ECode.Success;
            }

            long nowS = TimeUtils.GetTimeS();
            user.offlineTimeS = nowS;

            this.service.ss.SetDestroyTimer(user, UserDestroyUserReason.DestroyTimer_DisconnectFromGateway);
            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgUserDisconnectFromGateway msg, ECode e, ResUserDisconnectFromGateway res)
        {
            this.service.TryUnlockUser(msg.userId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}