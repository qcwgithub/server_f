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

            this.service.logger.InfoFormat("{0} userId {1}, reason {1}, preCount {2}", this.msgType, msg.userId, msg.reason, sd.userDict.Count);

            GatewayUser? user = sd.GetUser(msg.userId);
            if (user == null)
            {
                logger.InfoFormat("{0} user not exist, userId: {1}", this.msgType, msg.userId);
                return ECode.UserNotExist;
            }

            if (msg.msgKick != null && user.IsConnected())
            {
                user.connection.Send<MsgKick>(MsgType.Kick, msg.msgKick);
            }

            if (user.connection != null)
            {
                user.connection.Close(nameof(Gateway_DestroyUser));
            }

            if (user.destroyTimer.IsAlive())
            {
                this.service.ss.ClearDestroyTimer(user);
            }

            user.destroying = true;

            var msgUser = new MsgUserDestroyUser();
            msgUser.userId = msg.userId;
            msgUser.reason = nameof(Gateway_DestroyUser);

            var r = await this.service.connectToUserManagerService.Request<MsgUserDestroyUser, ResUserDestroyUser>(MsgType._User_DestroyUser, msgUser);
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            sd.userDict.Remove(msg.userId);
            return ECode.Success;
        }
    }
}