
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class User_DestroyPlayer : UserHandler
    {
        public override MsgType msgType => MsgType._User_DestroyUser;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgDestroyUser>(_msg);
            long userId = msg.userId;

            this.service.logger.InfoFormat("{0} place: {1}, userId: {2}, preCount: {3}", this.msgType, msg.place, userId, this.usData.userDict.Count);

            User? user = this.usData.GetUser(userId);
            if (user == null)
            {
                logger.InfoFormat("{0} user not exist, userId: {1}", this.msgType, userId);
                return ECode.UserNotExist;
            }

            if (msg.msgKick != null && user.IsSocketConnected())
            {
                user.socket.Send(MsgType.Kick, msg.msgKick, null, 0);
            }

            if (user.socket != null)
            {
                user.socket.Close("User_DestroyPlayer"); // PMOnDisconnect
            }

            if (user.destroyTimer.IsAlive())
            {
                this.service.usScript.ClearDestroyTimer(user, false);
            }

            // clear save timer
            if (user.saveTimer.IsAlive())
            {
                this.service.usScript.ClearSaveTimer(user);
            }

            user.destroying = true;

            // 保存一次
            var msgSave = new MsgSaveUser { userId = userId, place = this.msgType.ToString() };
            // this.service.ProxyDispatch(null, MsgType._PSSavePlayer, msgSave, null);
            MyResponse r = await this.service.connectToSelf.SendToSelfAsync(MsgType._User_SaveUser, msgSave);
            if (r.err != ECode.Success)
            {
                return r.err;
            }

            this.usData.userDict.Remove(userId);
            this.server.playerPSRedis.DeletePSId(userId);

            return ECode.Success;
        }
    }
}