using System.Net;
using System.Net.Sockets;
using System.Text;
using Data;

namespace Script
{
    public class UserManager_UserLogin : UserManagerHandler<MsgUserLogin, ResUserLogin>
    {
        public UserManager_UserLogin(Server server, UserManagerService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._UserManager_UserLogin;

        public override async Task<ECode> Handle(IConnection connection, MsgUserLogin msg, ResUserLogin res)
        {
            if (!MyChannels.IsValidChannel(msg.channel) || !this.server.data.serverConfig.generalConfig.allowChannels.Contains(msg.channel))
            {
                this.logger.ErrorFormat("{0} channel:{1}, channelUserId:{2} invalid channel!", this.msgType, msg.channel, msg.channelUserId);
                return ECode.InvalidChannel;
            }

            string addressFamily = msg.dict["$addressFamily"];
            string ip = msg.dict["$ip"];

            this.logger.Info($"{this.msgType} version {msg.version} platform {msg.platform}" +
                $" channel {msg.channel} channelUserId {msg.channelUserId} verifyData {msg.verifyData}" +
                $" addressFamily {addressFamily} ip {ip}");

            ECode e = ECode.Success;

            switch (msg.channel)
            {
                case MyChannels.uuid:
                    e = this.service.channelUuid.VeryfyAccount(msg);
                    break;
            }

            if (e != ECode.Success)
            {
                return e;
            }

            // 先锁了再往下走
            msg.dict["$lockValue"] = await this.server.lockRedis.LockAccount(msg.channel, msg.channelUserId, this.service.logger);
            if (msg.dict["$lockValue"] == null)
            {
                this.service.logger.ErrorFormat("{0} lock failed, channel {1} channelUserId {2}", this.msgType, msg.channel, msg.channelUserId);
                return ECode.RedisLockFail;
            }

            AccountInfo accountInfo = await this.server.accountInfoProxy.Get(this.service.connectToDbService, msg.channel, msg.channelUserId);
            if (accountInfo != null)
            {
                if (this.IsBlocked(accountInfo))
                {
                    return ECode.Blocked;
                }
            }
            else
            {
                accountInfo = this.NewAccountInfo(msg);
                await this.server.accountInfoProxy.Save(accountInfo);
            }

            long userId;
            if (accountInfo.userIds.Count == 0)
            {
                userId = this.service.userIdSnowflakeScript.NextUserId();

                UserInfo userInfo = this.service.ss.NewUserInfo(userId);
                e = await this.service.ss.InsertUserInfo(userInfo);
                if (e != ECode.Success)
                {
                    this.logger.Error($"Create user info {userId} e = {e}");
                    return e;
                }

                this.logger.Info($"Create user info {userId} e = {e}");
                accountInfo.userIds.Add(userId);
            }
            else
            {
                userId = accountInfo.userIds[0];
            }

            res.userId = userId;
            return ECode.Success;
        }

        bool IsBlocked(AccountInfo accountInfo)
        {
            long nowS = TimeUtils.GetTimeS();
            if (accountInfo.block && (accountInfo.unblockTime == -1 || accountInfo.unblockTime >= nowS))
            {
                this.service.logger.InfoFormat("{0} channel {1} channelUserId {2} blocked! leftTimeS {3}",
                    this.msgType,
                    accountInfo.channel, accountInfo.channelUserId,
                    accountInfo.unblockTime == -1 ? -1 : accountInfo.unblockTime - nowS);

                return true;
            }
            return false;
        }

        public AccountInfo NewAccountInfo(MsgUserLogin msg)
        {
            var accountInfo = AccountInfo.Ensure(null);
            accountInfo.platform = msg.platform;
            accountInfo.channel = msg.channel;
            accountInfo.channelUserId = msg.channelUserId;
            accountInfo.createTimeS = TimeUtils.GetTimeS();
            return accountInfo;
        }

        public override void PostHandle(IConnection connection, MsgUserLogin msg, ECode e, ResUserLogin res)
        {
            if (msg.dict["$lockValue"] != null)
            {
                this.server.lockRedis.UnlockAccount(msg.channel, msg.channelUserId, msg.dict["$lockValue"]).Forget(this.service);
            }
        }
    }
}