using Data;

namespace Script
{
    public class Gateway_DestroyUser : GatewayHandler<MsgGatewayDestroyUser, ResGatewayDestroyUser>
    {
        public Gateway_DestroyUser(Server server, GatewayService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Gateway_DestroyUser;

        public override async Task<ECode> Handle(IConnection connection, MsgGatewayDestroyUser msg, ResGatewayDestroyUser res)
        {
            var sd = this.service.sd;

            this.service.logger.InfoFormat("{0} userId {1}, reason {1}, preCount {2}", this.msgType, msg.userId, msg.reason, sd.userCount);

            GatewayUser? user = sd.GetUser(msg.userId);
            if (user == null)
            {
                logger.InfoFormat("{0} user not exist, userId: {1}", this.msgType, msg.userId);
                return ECode.Success;
            }

            if (msg.msgKick != null && user.IsConnected())
            {
                user.connection.Send<MsgKick>(MsgType.Kick, msg.msgKick);
            }

            if (user.IsConnected())
            {
                user.connection.Close("Gateway_DestroyUser");
            }

            this.service.ss.ClearDestroyTimer(user, GatewayClearDestroyTimerReason.Destroy);

            user.destroying = true;

            sd.RemoveUser(msg.userId);
            return ECode.Success;
        }
    }
}