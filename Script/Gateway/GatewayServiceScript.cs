using Data;

namespace Script
{
    public class GatewayServiceScript : ServiceScript<GatewayService>
    {
        public GatewayServiceScript(Server server, GatewayService service) : base(server, service)
        {
        }

        public bool IsTransferMsgType(MsgType msgType)
        {
            switch (msgType)
            {
                case MsgType.UserLogin:
                    return false;

                default:
                    return true;
            }
        }

        public stTryTransferResult TryTransfer(GatewayUserConnection connection, MsgType msgType, ArraySegment<byte> msg, Action<ECode, byte[]>? reply)
        {
            var result = new stTryTransferResult();

            if (!this.IsTransferMsgType(msgType))
            {
                result.normalDispatch = true;
                return result;
            }

            result.normalDispatch = false;

            GatewayUser? user = connection.user;
            if (user == null)
            {
                this.service.logger.Error("TryTrasfer() connection.user == null");
                return result;
            }

            if (user.userServiceId == 0)
            {
                this.service.logger.Error("TryTransfer() user.userServiceId == 0");
                return result;
            }

            IConnection? serviceConnection = this.service.data.GetOtherServiceConnection(user.userServiceId);
            if (serviceConnection == null || !serviceConnection.IsConnected())
            {
                this.service.logger.Error("TryTransfer() serviceConnection == null || !serviceConnection.IsConnected()");
                return result;
            }
            
            byte[] msgBytes = msg.ToArray();
            serviceConnection.SendBytes(msgType, msgBytes, (e, segment) =>
            {
                if (reply != null)
                {
                    reply(e, segment.ToArray());
                }
            },
            pTimeoutS: null);
            return result;
        }

        public void SetDestroyTimer(GatewayUser user)
        {
            MyDebug.Assert(!user.destroyTimer.IsAlive());

            var SEC = this.service.sd.destroyTimeoutS;
            this.service.logger.InfoFormat("SetDestroyTimer userId {0}", user.userId);

            user.destroyTimer = this.server.timerScript.SetTimer(
                this.service.serviceId,
                SEC, MsgType._Gateway_DestroyUser,
                new MsgGatewayDestroyUser { userId = user.userId, reason = nameof(SetDestroyTimer), msgKick = null});
        }

        public void ClearDestroyTimer(GatewayUser user)
        {
            MyDebug.Assert(user.destroyTimer.IsAlive());
            this.service.logger.InfoFormat("ClearDestroyTimer userId({0})", user.userId);

            server.timerScript.ClearTimer(user.destroyTimer);
            user.destroyTimer = null;
        }
    }
}