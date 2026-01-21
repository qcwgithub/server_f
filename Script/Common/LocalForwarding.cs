using Data;

namespace Script
{
    public static class LocalForwarding
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

        public static ServiceType? G_to_S(GatewayService gatewayService, GatewayUserConnection connection, MsgType msgType, object msg, LocalReplyCallback? reply)
        {
            ServiceType? serviceType = ShouldForwardClientMessage(msgType);
            if (serviceType == null)
            {
                return null;
            }

            long userId = connection.userId;
            if (userId == 0)
            {
                gatewayService.logger.Error("G_to_S connection.userId == 0");
                return serviceType;
            }

            GatewayUser? user = gatewayService.sd.GetUser(connection.userId);
            if (user == null)
            {
                gatewayService.logger.Error("G_to_S user == null");
                return serviceType;
            }

            if (user.userServiceId == 0)
            {
                gatewayService.logger.Error("G_to_S user.userServiceId == 0");
                return serviceType;
            }

            IServiceConnection? serviceConnection = gatewayService.data.GetOtherServiceConnection(user.userServiceId);
            if (serviceConnection == null || !serviceConnection.IsConnected())
            {
                gatewayService.logger.Error("G_to_S serviceConnection == null || !serviceConnection.IsConnected()");
                return serviceType;
            }

            gatewayService.logger.Info($"G_to_S {msgType} -> {serviceConnection.identifierString}");

            var msgForward = new MsgForwardLocal();
            msgForward.userId = user.userId;
            msgForward.userIds = null;
            msgForward.innerMsgType = msgType;
            msgForward.innerMsg = msg;

            serviceConnection.Send(MsgType.Forward, msgForward, reply);
            return serviceType;
        }

        public static bool G_from_S(GatewayService gatewayService, MsgType msgType, object msg, LocalReplyCallback? reply)
        {
            if (msgType != MsgType.Forward)
            {
                return false;
            }

            var msgForward = (MsgForwardLocal)msg;
            var innerMsg = msgForward.innerMsg;

            ReplyCallback? remoteReply = null;
            if (reply != null)
            {
                remoteReply = (e, resBytes) =>
                {
                    var res = MessageTypeConfigData.DeserializeRes(msgForward.innerMsgType, resBytes);
                    reply(e, res);
                };
            }

            if (msgForward.userId > 0)
            {
                SendToClient(gatewayService, msgForward.userId, msgForward.innerMsgType, innerMsg, remoteReply);
            }
            if (msgForward.userIds != null)
            {
                foreach (long userId in msgForward.userIds)
                {
                    SendToClient(gatewayService, userId, msgForward.innerMsgType, innerMsg, remoteReply);
                }
            }
            return true;
        }

        public static async Task<bool> S_from_G(UserService userService, IServiceConnection serviceConnection, MsgType msgType, object msg, LocalReplyCallback? reply)
        {
            if (msgType != MsgType.Forward)
            {
                return false;
            }

            MyDebug.Assert(serviceConnection.knownWho);
            MyDebug.Assert(serviceConnection.serviceType == ServiceType.Gateway);

            var msgForward = (MsgForwardLocal)msg;
            var innerMsg = msgForward.innerMsg;

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
                reply(r.e, r.res);
            }

            return true;
        }

        public static void S_to_G(IServiceConnection serviceConnection, long userId, MsgType msgType, object msg, LocalReplyCallback? reply)
        {
            var msgForward = new MsgForwardLocal();
            msgForward.userId = userId;
            msgForward.userIds = null;
            msgForward.innerMsgType = msgType;
            msgForward.innerMsg = msg;

            S_to_G(serviceConnection, msgForward, reply);
        }

        public static void S_to_G(IServiceConnection serviceConnection, long[] userIds, MsgType msgType, object msg, LocalReplyCallback? reply)
        {
            var msgForward = new MsgForwardLocal();
            msgForward.userId = 0;
            msgForward.userIds = userIds;
            msgForward.innerMsgType = msgType;
            msgForward.innerMsg = msg;

            S_to_G(serviceConnection, msgForward, reply);
        }

        static void S_to_G(IServiceConnection serviceConnection, MsgForwardLocal msgForward, LocalReplyCallback? reply)
        {
            MyDebug.Assert(serviceConnection.knownWho);
            MyDebug.Assert(serviceConnection.serviceType == ServiceType.Gateway);

            serviceConnection.Send(MsgType.Forward, msgForward, reply);
        }

        static void SendToClient(GatewayService gatewayService, long userId, MsgType msgType, object msg, ReplyCallback? reply)
        {
            GatewayUser? user = gatewayService.sd.GetUser(userId);
            if (user == null || user.connection == null || !user.connection.IsConnected())
            {
                return;
            }

            user.connection.Send(msgType, msg, reply);
        }
    }
}