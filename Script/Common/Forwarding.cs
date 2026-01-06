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

        public static ServiceType? G_to_S(GatewayService gatewayService, GatewayUserConnection connection, MsgType msgType, ArraySegment<byte> msgBytes, ReplyCallback? reply)
        {
            ServiceType? serviceType = ShouldForwardClientMessage(msgType);
            if (serviceType == null)
            {
                gatewayService.logger.Info($"G_to_S {msgType} Not Forward");
                return null;
            }

            GatewayUser? user = connection.user;
            if (user == null)
            {
                gatewayService.logger.Error("G_to_S connection.user == null");
                return serviceType;
            }

            if (user.userServiceId == 0)
            {
                gatewayService.logger.Error("G_to_S user.userServiceId == 0");
                return serviceType;
            }

            ServiceConnection? serviceConnection = gatewayService.data.GetOtherServiceConnection(user.userServiceId);
            if (serviceConnection == null || !serviceConnection.IsConnected())
            {
                gatewayService.logger.Error("G_to_S serviceConnection == null || !serviceConnection.IsConnected()");
                return serviceType;
            }

            gatewayService.logger.Info($"G_to_S {msgType} -> {serviceConnection.tai}");

            var msgForward = new MsgForward();
            msgForward.userId = user.userId;
            msgForward.userIds = null;
            msgForward.innerMsgType = msgType;
            msgForward.innerMsgBytes = msgBytes;

            serviceConnection.Send(MsgType.Forward, msgForward, reply, null);
            return serviceType;
        }

        public static bool G_from_S(GatewayService gatewayService, MsgType msgType, ArraySegment<byte> msgBytes, ReplyCallback? reply)
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

        public static async Task<bool> S_from_G(UserService userService, ServiceConnection serviceConnection, MsgType msgType, ArraySegment<byte> msgBytes, ReplyCallback? reply)
        {
            if (msgType != MsgType.Forward)
            {
                userService.logger.Info($"S_from_G {msgType} Not Forward");
                return false;
            }

            MyDebug.Assert(serviceConnection.serviceType == ServiceType.Gateway);

            var msgForward = (MsgForward)MessageTypeConfigData.DeserializeMsg(msgType, msgBytes);
            var innerMsg = MessageTypeConfigData.DeserializeMsg(msgForward.innerMsgType, msgForward.innerMsgBytes);

            User? user = userService.sd.GetUser(msgForward.userId);

            if (user == null)
            {
                userService.logger.Error($"S_from_G {msgType} user == null");
                return true;
            }

            if (user.connection == null)
            {
                userService.logger.Error($"S_from_G {msgType} user.connection == null");
                return true;
            }

            if (user.connection.gatewayServiceId != serviceConnection.serviceId)
            {
                userService.logger.Error($"S_from_G {msgType} user.connection.gatewayServiceId {user.connection.gatewayServiceId} != serviceConnection.serviceId {serviceConnection.serviceId}");
                return true;
            }

            var context = new MessageContext
            {
                connection = serviceConnection,
                msg_userId = msgForward.userId
            };

            userService.logger.Info($"S_from_G {msgForward.innerMsgType}");

            var r = await userService.dispatcher.Dispatch(context, msgForward.innerMsgType, innerMsg);
            if (reply != null)
            {
                ArraySegment<byte> resBytes = MessageTypeConfigData.SerializeRes(msgForward.innerMsgType, r.res);
                reply(r.e, resBytes);
            }

            return true;
        }

        public static void S_to_G(ServiceConnection serviceConnection, long userId, MsgType msgType, object msg, ReplyCallback? reply, int? pTimeoutS)
        {
            var msgForward = new MsgForward();
            msgForward.userId = userId;
            msgForward.userIds = null;
            msgForward.innerMsgType = msgType;
            msgForward.innerMsgBytes = MessageTypeConfigData.SerializeMsg(msgType, msg);

            S_to_G(serviceConnection, msgForward, reply, pTimeoutS);
        }

        public static void S_to_G(ServiceConnection serviceConnection, List<long> userIds, MsgType msgType, object msg, ReplyCallback? reply, int? pTimeoutS)
        {
            var msgForward = new MsgForward();
            msgForward.userId = 0;
            msgForward.userIds = userIds;
            msgForward.innerMsgType = msgType;
            msgForward.innerMsgBytes = MessageTypeConfigData.SerializeMsg(msgType, msg);

            S_to_G(serviceConnection, msgForward, reply, pTimeoutS);
        }

        static void S_to_G(ServiceConnection serviceConnection, MsgForward msgForward, ReplyCallback? reply, int? pTimeoutS)
        {
            MyDebug.Assert(serviceConnection.serviceType == ServiceType.Gateway);

            serviceConnection.Send(MsgType.Forward, msgForward, reply, pTimeoutS);
        }

        static void SendToClient(GatewayService gatewayService, long userId, MsgType msgType, object msg, ReplyCallback? reply)
        {
            GatewayUser? user = gatewayService.sd.GetUser(userId);
            if (user == null || user.connection == null || !user.connection.IsConnected())
            {
                return;
            }

            user.connection.Send(msgType, msg, reply, null);
        }
    }
}