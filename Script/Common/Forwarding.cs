using Data;

namespace Script
{
    public static class Forwarding
    {
        public static ServiceType? ShouldForwardClientMessage(MsgType msgType)
        {
            switch (msgType)
            {
                case MsgType.Login:
                    return null;

                default:
                    return ServiceType.User;
            }
        }

        // G:C->S
        public static ServiceType? GatewayTryForwardClientMessageToOtherService(GatewayService gatewayService, GatewayUserConnection connection, MsgType msgType, ArraySegment<byte> msg, ReplyCallback? reply)
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

            if (reply != null)
            {
                serviceConnection.SendBytes(msgType, msgBytes, (e, segment) =>
                {
                    reply(e, segment.ToArray());
                },
                null);
            }
            else
            {
                serviceConnection.SendBytes(msgType, msgBytes, null, null);
            }
            return serviceType;
        }

        // S:<-G<-C
        public static async Task<bool> TryReceiveClientMessageFromGateway(UserService userService, ServiceConnection serviceConnection, MsgType msgType, ArraySegment<byte> msgBytes, ReplyCallback? reply)
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

            var context = new MsgContext
            {
                connection = serviceConnection,
                user = user
            };
            (ECode e, ArraySegment<byte> resBytes) = await userService.dispatcher.Dispatch(context, msgType, msgBytes2);
            if (reply != null)
            {
                reply(e, resBytes);
            }

            return true;
        }

        // S:->G->C
        public static void SendClientMessageThroughGateway(ServiceConnection serviceConnection, long userId, MsgType msgType, byte[] msg, ReplyCallback reply, int? pTimeoutS)
        {
            MyDebug.Assert(serviceConnection.serviceType == ServiceType.Gateway);

            // +userId
            byte[] msgBytes = new byte[8 + msg.Length];
            BinaryMessagePacker.WriteLong(msgBytes, 0, userId);
            msg.CopyTo(msgBytes, 8);

            if (reply != null)
            {
                serviceConnection.SendBytes(msgType, msgBytes, (e, segment) =>
                {
                    reply(e, segment.ToArray());
                },
                pTimeoutS);
            }
            else
            {
                serviceConnection.SendBytes(msgType, msgBytes, null, null);
            }
        }

        // G:S->C
        public static bool GatewayTryForwardClientMessageToClient(GatewayService gatewayService, MsgType msgType, ArraySegment<byte> msgBytes, ReplyCallback? reply)
        {
            if (!msgType.IsClient())
            {
                return false;
            }

            long userId = BinaryMessagePacker.ReadLong(msgBytes, 0);
            var msgBytes2 = new ArraySegment<byte>(msgBytes.Array!, msgBytes.Offset + 8, msgBytes.Count - 8);

            GatewayUser? user = gatewayService.sd.GetUser(userId);
            if (user == null || user.connection == null || !user.connection.IsConnected())
            {
                return true;
            }

            if (reply != null)
            {
                user.connection.SendBytes(msgType, msgBytes2.ToArray(), (e, segment) =>
                {
                    reply(e, segment.ToArray());
                },
                null);
            }
            else
            {
                user.connection.SendBytes(msgType, msgBytes2.ToArray(), null, null);
            }
            return true;
        }
    }
}