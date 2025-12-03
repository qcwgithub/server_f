
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class User_OnSocketClose : OnSocketClose<UserService>
    {
        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            await base.Handle(socket, _msg);

            var msg = (MsgOnClose)_msg;
            // if (msg.isServer)
            // {
            //     return ECode.Success;
            // }

            if (socket.oppositeIsClient)
            {
                var user = (User?)this.service.tcpClientScript.GetUser(socket);
                if (user == null)
                {
                    return ECode.Success;
                }

                this.service.logger.InfoFormat("{0} userId {1} closeReason {2}", this.msgType, user.userId, socket.closeReason);

                if (user.socket != null)
                {
                    this.service.tcpClientScript.UnbindUser(user.socket, user);
                }

                int nowS = TimeUtils.GetTimeS();
                user.offlineTimeS = nowS;

                // this.service.sqlLog.PlayerLogout(player);

                if (!user.destroyTimer.IsAlive())
                {
                    this.service.usScript.SetDestroyTimer(user, this.msgType.ToString());
                }
            }

            return ECode.Success;
        }
    }
}