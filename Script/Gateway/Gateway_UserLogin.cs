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
            var userConnection = (GatewayUserConnection)connection;

            if (msg.dict == null)
            {
                msg.dict = new Dictionary<string, string>();
            }

            (AddressFamily family, string ip) = this.GetIp(userConnection.socket);

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

            var msgU = new MsgUserLoginSuccess();
            msgU.isNewUser = rUM.res.isNewUser;
            msgU.userId = rUM.res.userId;
            msgU.newUserInfo = rUM.res.newUserInfo;

            var rU = await this.service.connectToUserService.Request<MsgUserLoginSuccess, ResUserLoginSuccess>(MsgType._User_UserLoginSuccess, msgU);
            if (rU.e != ECode.Success)
            {
                return rU.e;
            }

            res.userInfo = rU.res.userInfo;
            res.kickOther = rU.res.kickOther;
            res.delayS = rU.res.delayS;
            return ECode.Success;
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