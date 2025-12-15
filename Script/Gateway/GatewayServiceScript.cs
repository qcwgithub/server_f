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

        public stTryTransferResult TryTransfer(GatewayClientConnection connection, MsgType msgType, ArraySegment<byte> msg, Action<ECode, byte[]>? reply)
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
    }
}