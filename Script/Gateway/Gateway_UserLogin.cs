using System.Net;
using System.Net.Sockets;
using Data;

namespace Script
{
    public class Gateway_UserLogin : GatewayHandler<MsgUserLogin, ResUserLogin>
    {
        public Gateway_UserLogin(Server server, GatewayService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType.UserLogin;

        public override async Task<ECode> Handle(IConnection connection, MsgUserLogin msg, ResUserLogin res)
        {
            var undefinedConnection = (UndefinedConnection)connection;
            ProtocolClientData socket = undefinedConnection.socket;

            (AddressFamily family, string ip) = this.GetIp(socket);

            //------------------------------------------------------------------------------

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

            var rUM = await this.service.connectToUserManagerService.Request<MsgUserManagerUserLogin, ResUserManagerUserLogin>(MsgType._UserManager_UserLogin, msgUM);
            if (rUM.e != ECode.Success)
            {
                return rUM.e;
            }

            ResUserManagerUserLogin resUM = rUM.res;

            //------------------------------------------------------------------------------

            GatewayUser? user = this.service.sd.GetUser(resUM.userId);
            int userServiceId;
            if (user != null)
            {
                userServiceId = user.userServiceId;
            }
            else
            {
                userServiceId = await this.server.userUSRedis.GetUSId(resUM.userId);
                if (userServiceId == 0)
                {
                    userServiceId = await this.service.userServiceAllocator.AllocUserServiceId(resUM.userId);
                    if (userServiceId == 0)
                    {
                        return ECode.NoAvailableUserService;
                    }
                }
            }

            var msgU = new MsgUserLoginSuccess();
            msgU.isNewUser = resUM.isNewUser;
            msgU.userId = resUM.userId;
            msgU.newUserInfo = resUM.newUserInfo;

            var rU = await this.service.connectToUserService.Request<MsgUserLoginSuccess, ResUserLoginSuccess>(userServiceId, MsgType._User_UserLoginSuccess, msgU);
            if (rU.e != ECode.Success)
            {
                return rU.e;
            }

            ResUserLoginSuccess resU = rU.res;

            //------------------------------------------------------------------------------

            res.userInfo = resU.userInfo;
            res.kickOther = resU.kickOther;
            res.delayS = resU.delayS;

            if (user != null)
            {
                this.HandleOldConnection(user);
            }
            else
            {
                user = new GatewayUser(resUM.userId, userServiceId);
                this.service.sd.AddUser(user);
            }

            user.connection = new GatewayUserConnection(socket, user);
            return ECode.Success;
        }

        bool HandleOldConnection(GatewayUser user)
        {
            if (!user.IsConnected())
            {
                return false;
            }

            GatewayUserConnection oldConnection = user.connection;
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

                IPEndPoint ipEndPoint = endPoint as IPEndPoint;
                if (ipEndPoint != null)
                {
                    ip = ipEndPoint.Address.MapToIPv4().ToString();
                }
            }

            return (addressFamily, ip);
        }
    }
}