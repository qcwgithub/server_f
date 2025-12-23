using Data;

namespace Script
{
    public static class Forwarding
    {
        public static ServiceType? ShouldForwardClientMessage(MsgType msgType)
        {
            switch (msgType)
            {
                case MsgType.UserLogin:
                    return null;

                default:
                    return ServiceType.User;
            }
        }

        public static ServiceType? TryForwardClientMessage(GatewayService gatewayService, GatewayUserConnection connection, MsgType msgType, ArraySegment<byte> msg, Action<ECode, byte[]>? reply)
        {
            ServiceType? serviceType = ShouldForwardClientMessage(msgType);
            if (serviceType == null)
            {
                return serviceType;
            }

            GatewayUser? user = connection.user;
            if (user == null)
            {
                gatewayService.logger.Error("TryForwardToOtherService() connection.user == null");
                return serviceType;
            }

            if (user.userServiceId == 0)
            {
                gatewayService.logger.Error("TryForwardToOtherService() user.userServiceId == 0");
                return serviceType;
            }

            IConnection? serviceConnection = gatewayService.data.GetOtherServiceConnection(user.userServiceId);
            if (serviceConnection == null || !serviceConnection.IsConnected())
            {
                gatewayService.logger.Error("TryForwardToOtherService() serviceConnection == null || !serviceConnection.IsConnected()");
                return serviceType;
            }

            // +userId
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
            return serviceType;
        }

        public static async Task<bool> TryReceiveClientMessageFromGateway(UserService userService, ServiceConnection serviceConnection, MsgType msgType, ArraySegment<byte> msgBytes, Action<ECode, byte[]>? reply)
        {
            if (!msgType.IsClient() ||
                serviceConnection.serviceType != ServiceType.Gateway)
            {
                return false;
            }

            long userId = BinaryMessagePacker.ReadLong(msgBytes, 0);
            var msgBytes2 = new ArraySegment<byte>(msgBytes.Array!, msgBytes.Offset + 8, msgBytes.Count - 8);

            User? user = userService.sd.GetUser(userId);

            MyDebug.Assert(user != null);
            MyDebug.Assert(user.connection != null);
            MyDebug.Assert(user.connection.gatewayServiceId == serviceConnection.serviceId);

            (ECode e, byte[] resBytes) = await userService.dispatcher.Dispatch(user.connection, msgType, msgBytes2);
            if (reply != null)
            {
                reply(e, resBytes);
            }

            return true;
        }

        public static void SendClientMessageThroughGateway(ServiceConnection serviceConnection, long userId, MsgType msgType, byte[] msg, Action<ECode, ArraySegment<byte>>? cb, int? pTimeoutS)
        {
            MyDebug.Assert(serviceConnection.serviceType == ServiceType.Gateway);

            // +userId
            byte[] msgBytes = new byte[8 + msg.Length];
            BinaryMessagePacker.WriteLong(msgBytes, 0, userId);
            msg.CopyTo(msgBytes, 8);

            serviceConnection.socket.SendBytes(msgType, msgBytes, cb, pTimeoutS);
        }
    }
}