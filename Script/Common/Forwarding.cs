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
        public static ServiceType? GatewayTryForwardClientMessageToOtherService(GatewayService gatewayService, GatewayUserConnection connection, MsgType msgType, ArraySegment<byte> msgBytes, ReplyCallback? reply)
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

            var msgForward = new MsgForward();
            msgForward.userId = user.userId;
            msgForward.userIds = null;
            msgForward.innerMsgType = msgType;
            msgForward.innerMsgBytes = msgBytes;

            if (reply != null)
            {
                serviceConnection.Send(MsgType.Forward, msgForward, (e, segment) =>
                {
                    reply(e, segment.ToArray());
                },
                null);
            }
            else
            {
                serviceConnection.Send(MsgType.Forward, msgForward, null, null);
            }
            return serviceType;
        }

        // G:S->C
        public static bool GatewayTryForwardClientMessageToClient(GatewayService gatewayService, MsgType msgType, ArraySegment<byte> msgBytes, ReplyCallback? reply)
        {
            if (msgType != MsgType.Forward)
            {
                return false;
            }

            var msgForward = (MsgForward)MessageTypeConfigData.DeserializeMsg(msgType, msgBytes);
            var innerMsg = MessageTypeConfigData.DeserializeMsg(msgForward.innerMsgType, msgForward.innerMsgBytes);

            if (msgForward.userId > 0)
            {
                SendToClient(gatewayService, msgForward.userId, msgForward.innerMsgType, innerMsg, reply);
            }
            if (msgForward.userIds != null)
            {
                foreach (long userId in msgForward.userIds)
                {
                    SendToClient(gatewayService, userId, msgForward.innerMsgType, innerMsg, reply);
                }
            }
            return true;
        }

        // S:<-G<-C
        public static async Task<bool> TryReceiveClientMessageFromGateway(UserService userService, ServiceConnection serviceConnection, MsgType msgType, ArraySegment<byte> msgBytes, ReplyCallback? reply)
        {
            if (msgType != MsgType.Forward)
            {
                return false;
            }

            MyDebug.Assert(serviceConnection.serviceType == ServiceType.Gateway);

            var msgForward = (MsgForward)MessageTypeConfigData.DeserializeMsg(msgType, msgBytes);
            var innerMsg = MessageTypeConfigData.DeserializeMsg(msgForward.innerMsgType, msgForward.innerMsgBytes);

            User? user = userService.sd.GetUser(msgForward.userId);

            if (user == null)
            {
                MyDebug.Assert(false);
                return true;
            }

            if (user.connection == null)
            {
                MyDebug.Assert(false);
                return true;
            }

            if (user.connection.gatewayServiceId != serviceConnection.serviceId)
            {
                MyDebug.Assert(false);
                return true;
            }

            var context = new MessageContext
            {
                connection = serviceConnection,
                msg_userId = msgForward.userId
            };

            var r = await userService.dispatcher.Dispatch(context, msgForward.innerMsgType, innerMsg);
            if (reply != null)
            {
                ArraySegment<byte> resBytes = default;
                if (r.res != null)
                {
                    resBytes = MessageTypeConfigData.SerializeRes(msgType, r.res);
                }

                reply(r.e, resBytes);
            }

            return true;
        }

        // S:->G->C
        public static void SendClientMessageThroughGateway(ServiceConnection serviceConnection, long userId, MsgType msgType, object msg, ReplyCallback reply, int? pTimeoutS)
        {
            var msgForward = new MsgForward();
            msgForward.userId = userId;
            msgForward.userIds = null;
            msgForward.innerMsgType = msgType;
            msgForward.innerMsgBytes = MessageTypeConfigData.SerializeMsg(msgType, msg);

            SendClientMessageThroughGateway(serviceConnection, msgForward, reply, pTimeoutS);
        }

        public static void SendClientMessageThroughGateway(ServiceConnection serviceConnection, List<long> userIds, MsgType msgType, object msg, ReplyCallback reply, int? pTimeoutS)
        {
            var msgForward = new MsgForward();
            msgForward.userId = 0;
            msgForward.userIds = userIds;
            msgForward.innerMsgType = msgType;
            msgForward.innerMsgBytes = MessageTypeConfigData.SerializeMsg(msgType, msg);

            SendClientMessageThroughGateway(serviceConnection, msgForward, reply, pTimeoutS);
        }

        static void SendClientMessageThroughGateway(ServiceConnection serviceConnection, MsgForward msgForward, ReplyCallback reply, int? pTimeoutS)
        {
            MyDebug.Assert(serviceConnection.serviceType == ServiceType.Gateway);

            if (reply != null)
            {
                serviceConnection.Send(MsgType.Forward, msgForward, (e, segment) =>
                {
                    reply(e, segment.ToArray());
                },
                pTimeoutS);
            }
            else
            {
                serviceConnection.Send(MsgType.Forward, msgForward, null, null);
            }
        }

        static void SendToClient(GatewayService gatewayService, long userId, MsgType msgType, object msg, ReplyCallback? reply)
        {
            GatewayUser? user = gatewayService.sd.GetUser(userId);
            if (user == null || user.connection == null || !user.connection.IsConnected())
            {
                return;
            }

            if (reply != null)
            {
                user.connection.Send(msgType, msg, (e, segment) =>
                {
                    reply(e, segment.ToArray());
                },
                null);
            }
            else
            {
                user.connection.Send(msgType, msg, null, null);
            }
        }
    }
}