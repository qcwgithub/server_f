using Data;

namespace Script
{
    public class GatewayProtocolClientScriptForC : ProtocolClientScript
    {
        public GatewayProtocolClientScriptForC(Server server, Service service) : base(server, service)
        {
        }

        public GatewayService gatewayService
        {
            get
            {
                return (GatewayService)this.service;
            }
        }

        public static bool IsTransferToSMsgType(MsgType msgType)
        {
            switch (msgType)
            {
                case MsgType.UserLogin:
                    return false;

                default:
                    return true;
            }
        }

        stGatewayTransferToSResult TransferToS(GatewayUserConnection connection, MsgType msgType, ArraySegment<byte> msg, Action<ECode, byte[]>? reply)
        {
            var result = new stGatewayTransferToSResult();

            if (!IsTransferToSMsgType(msgType))
            {
                result.transferred = false;
                return result;
            }

            result.transferred = true;

            GatewayUser? user = connection.user;
            if (user == null)
            {
                this.service.logger.Error("TransferToS() connection.user == null");
                return result;
            }

            if (user.userServiceId == 0)
            {
                this.service.logger.Error("TransferToS() user.userServiceId == 0");
                return result;
            }

            IConnection? serviceConnection = this.service.data.GetOtherServiceConnection(user.userServiceId);
            if (serviceConnection == null || !serviceConnection.IsConnected())
            {
                this.service.logger.Error("TransferToS() serviceConnection == null || !serviceConnection.IsConnected()");
                return result;
            }

            // +user.userId
            byte[] msgBytes = new byte[8 + msg.Count];
            BinaryMessagePacker.WriteLong(msgBytes, 0, user.userId);
            msg.CopyTo(msgBytes, 8);

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

        public override void DispatchNetwork(ProtocolClientData data, int seq, MsgType msgType, ArraySegment<byte> msgBytes, Action<ECode, byte[]>? reply)
        {
            var connection = (GatewayUserConnection)data.customData;

            stGatewayTransferToSResult result = this.TransferToS(connection, msgType, msgBytes, reply);
            if (!result.transferred)
            {
                base.DispatchNetwork(data, seq, msgType, msgBytes, reply);
            }
        }
    }
}