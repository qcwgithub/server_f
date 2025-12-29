using System.Net;
using System.Net.Sockets;
using Data;

namespace Script
{
    public class Gateway_UserLogin : GatewayHandler<MsgLogin, ResLogin>
    {
        public Gateway_UserLogin(Server server, GatewayService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType.Login;

        public override async Task<ECode> Handle(MsgContext context, MsgLogin msg, ResLogin res)
        {
            ProtocolClientData socket = (context.connection as PendingSocketConnection).socket;

            (AddressFamily family, string ip) = this.GetIp(socket);

            ////

            var msgUM = new MsgUserManagerUserLogin();
            msgUM.version = msg.version;
            msgUM.platform = msg.platform;
            msgUM.channel = msg.channel;
            msgUM.channelUserId = msg.channelUserId;
            msgUM.verifyData = msg.verifyData;
            msgUM.token = msg.token;
            msgUM.deviceUid = msg.deviceUid;
            msgUM.addressFamily = family.ToString();
            msgUM.ip = ip;

            var rUM = await this.service.connectToUserManagerService.UserLogin(msgUM);
            if (rUM.e != ECode.Success)
            {
                return rUM.e;
            }

            ResUserManagerUserLogin resUM = rUM.res;

            ////

            GatewayUser? user = this.service.sd.GetUser(resUM.userInfo.userId);
            if (user != null)
            {
                this.HandleOldConnection(user);
            }
            else
            {
                user = new GatewayUser(resUM.userInfo.userId, resUM.userServiceId);
                this.service.sd.AddUser(user);
            }

            user.connection = new GatewayUserConnection(socket, user);

            ////

            res.isNewUser = resUM.isNewUser;
            res.userInfo = resUM.userInfo;
            res.kickOther = resUM.kickOther;
            res.delayS = resUM.delayS;
            return ECode.Success;
        }

        bool HandleOldConnection(GatewayUser user)
        {
            GatewayUserConnection? oldConnection = user.connection;
            if (oldConnection == null || !oldConnection.IsConnected())
            {
                return false;
            }

            this.service.logger.Info($"userId {user.userId} kick old");

            user.connection = null;

            // Case 1
            // User connect to the same Gateway twice
            var msgKick = new MsgKick();
            msgKick.flags = LogoutFlags.CancelAutoLogin;
            oldConnection.Send<MsgKick>(MsgType.Kick, msgKick, null, null);

            return true;
        }

        (AddressFamily, string) GetIp(ProtocolClientData socket)
        {
            EndPoint endPoint = socket.RemoteEndPoint;
            AddressFamily addressFamily = AddressFamily.Unknown;
            string ip = string.Empty;
            if (endPoint != null)
            {
                addressFamily = endPoint.AddressFamily;

                IPEndPoint? ipEndPoint = endPoint as IPEndPoint;
                if (ipEndPoint != null)
                {
                    ip = ipEndPoint.Address.MapToIPv4().ToString();
                }
            }

            return (addressFamily, ip);
        }
    }
}