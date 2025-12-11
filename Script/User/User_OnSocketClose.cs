
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class User_OnSocketClose : OnSocketClose<UserService>
    {
        public User_OnSocketClose(Server server, UserService service) : base(server, service)
        {
        }


        public override async Task<ECode> Handle(IConnection connection, MsgConnectionClose msg, ResConnectionClose res)
        {
            await base.Handle(connection, msg);

            // if (msg.isServer)
            // {
            //     return ECode.Success;
            // }

            if (connection.oppositeIsClient)
            {
                var user = (User?)this.service.tcpClientScript.GetUser(connection);
                if (user == null)
                {
                    return ECode.Success;
                }

                this.service.logger.InfoFormat("{0} userId {1} closeReason {2}", this.msgType, user.userId, connection.closeReason);

                if (user.connection != null)
                {
                    this.service.tcpClientScript.UnbindUser(user.connection, user);
                }

                long nowS = TimeUtils.GetTimeS();
                user.offlineTimeS = nowS;

                // this.service.sqlLog.PlayerLogout(player);

                if (!user.destroyTimer.IsAlive())
                {
                    this.service.ss.SetDestroyTimer(user, this.msgType.ToString());
                }
            }

            return ECode.Success;
        }
    }
}