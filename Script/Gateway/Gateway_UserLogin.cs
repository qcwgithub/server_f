using System.Net;
using System.Net.Sockets;
using Data;

namespace Script
{
    [AutoRegister]
    public class Gateway_UserLogin : Handler<GatewayService, MsgLogin, ResLogin>
    {
        public Gateway_UserLogin(Server server, GatewayService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType.Login;

        public override async Task<ECode> Handle(MessageContext context, MsgLogin msg, ResLogin res)
        {
            (AddressFamily family, string ip) = this.GetIp(context.connection);

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

            var r = await this.service.userManagerServiceProxy.UserLogin(msgUM);
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            var resUM = r.CastRes<ResUserManagerUserLogin>();

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

            user.connection = (GatewayUserConnection)context.connection;
            user.connection.userId = user.userId;

            ////

            res.isNewUser = resUM.isNewUser;
            res.userInfo = resUM.userInfo;
            res.kickOther = resUM.kickOther;
            return ECode.Success;
        }

        bool HandleOldConnection(GatewayUser user)
        {
            GatewayUserConnection? oldConnection = user.connection;
            if (oldConnection == null)
            {
                return false;
            }

            user.connection = null;
            oldConnection.userId = 0;

            this.service.logger.Info($"userId {user.userId} kick old");

            // Case 1
            // User connect to the same Gateway twice
            var msgKick = new MsgKick();
            msgKick.flags = LogoutFlags.CancelAutoLogin;
            oldConnection.Send(MsgType.Kick, msgKick, null);

            return true;
        }

        (AddressFamily, string) GetIp(IConnection connection)
        {
            return (default, "Unknown!");
            // EndPoint endPoint = socket.RemoteEndPoint;
            // AddressFamily addressFamily = AddressFamily.Unknown;
            // string ip = string.Empty;
            // if (endPoint != null)
            // {
            //     addressFamily = endPoint.AddressFamily;

            //     IPEndPoint? ipEndPoint = endPoint as IPEndPoint;
            //     if (ipEndPoint != null)
            //     {
            //         ip = ipEndPoint.Address.MapToIPv4().ToString();
            //     }
            // }

            // return (addressFamily, ip);
        }
    }
}