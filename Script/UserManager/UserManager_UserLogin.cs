using System.Net;
using System.Net.Sockets;
using System.Text;
using Data;

namespace Script
{
    public class UserManager_UserLogin : UserManagerHandler<MsgUserManagerUserLogin, ResUserManagerUserLogin>
    {
        public UserManager_UserLogin(Server server, UserManagerService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._UserManager_UserLogin;

        public override async Task<ECode> Handle(MessageContext context, MsgUserManagerUserLogin msg, ResUserManagerUserLogin res)
        {
            if (!MyChannels.IsValidChannel(msg.channel) || !this.server.data.serverConfig.generalConfig.allowChannels.Contains(msg.channel))
            {
                this.service.logger.ErrorFormat("{0} channel:{1}, channelUserId:{2} invalid channel!", this.msgType, msg.channel, msg.channelUserId);
                return ECode.InvalidChannel;
            }

            this.service.logger.Info($"{this.msgType} version {msg.version} platform {msg.platform}" +
                $" channel {msg.channel} channelUserId {msg.channelUserId} verifyData {msg.verifyData}" +
                $" addressFamily {msg.addressFamily} ip {msg.ip}");

            ECode e = ECode.Success;

            switch (msg.channel)
            {
                case MyChannels.uuid:
                    e = this.service.channelUuid.VeryfyAccount(msg.platform, msg.channel, msg.channelUserId);
                    break;
            }

            if (e != ECode.Success)
            {
                return e;
            }

            context.lockValue = await this.server.lockRedis.LockAccount(msg.channel, msg.channelUserId, this.service.logger);
            if (context.lockValue == null)
            {
                this.service.logger.ErrorFormat("{0} lock failed, channel {1} channelUserId {2}", this.msgType, msg.channel, msg.channelUserId);
                return ECode.RedisLockFail;
            }

            AccountInfo accountInfo = await this.server.accountInfoProxy.Get(this.service.dbServiceProxy, msg.channel, msg.channelUserId);
            if (accountInfo != null)
            {
                if (this.IsBlocked(accountInfo))
                {
                    return ECode.Blocked;
                }
            }
            else
            {
                accountInfo = this.NewAccountInfo(msg.platform, msg.channel, msg.channelUserId);
                await this.server.accountInfoProxy.Save(accountInfo);
            }

            bool isNewUser;
            long userId;
            UserInfo? newUserInfo;

            if (accountInfo.userIds.Count == 0)
            {
                isNewUser = true;
                userId = this.service.userIdSnowflakeScript.NextUserId();
                newUserInfo = this.service.ss.NewUserInfo(userId);

                e = await this.service.ss.InsertUserInfo(newUserInfo);
                if (e != ECode.Success)
                {
                    this.service.logger.Error($"Create user info {userId} {e}");
                    return e;
                }

                this.service.logger.Info($"Create user info {userId} {e}");
                accountInfo.userIds.Add(userId);
            }
            else
            {
                isNewUser = false;
                userId = accountInfo.userIds[0];
                newUserInfo = null;
            }

            ////

            stObjectLocation location = await this.service.userLocator.GetLocation(userId);
            if (!location.IsValid())
            {
                location = await this.service.userLocationAssignmentScript.AssignLocation(userId);
                if (!location.IsValid())
                {
                    return ECode.NoAvailableUserService;
                }

                this.service.userLocator.CacheLocation(userId, location);
            }

            ////

            var msgU = new MsgUserLoginSuccess();
            msgU.isNewUser = isNewUser;
            msgU.userId = userId;
            msgU.newUserInfo = newUserInfo;
            msgU.gatewayServiceId = (context.connection as ServiceConnection).serviceId;

            var rU = await this.service.userServiceProxy.UserLoginSuccess(location.serviceId, msgU);
            if (rU.e != ECode.Success)
            {
                return rU.e;
            }

            var resU = rU.CastRes<ResUserLoginSuccess>();

            ////

            res.isNewUser = isNewUser;
            res.userInfo = resU.userInfo;
            res.kickOther = resU.kickOther;
            res.userServiceId = location.serviceId;

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

        public AccountInfo NewAccountInfo(string platform, string channel, string channelUserId)
        {
            var accountInfo = AccountInfo.Ensure(null);
            accountInfo.platform = platform;
            accountInfo.channel = channel;
            accountInfo.channelUserId = channelUserId;
            accountInfo.createTimeS = TimeUtils.GetTimeS();
            return accountInfo;
        }

        public override void PostHandle(MessageContext context, MsgUserManagerUserLogin msg, ECode e, ResUserManagerUserLogin res)
        {
            if (context.lockValue != null)
            {
                this.server.lockRedis.UnlockAccount(msg.channel, msg.channelUserId, context.lockValue).Forget(this.service);
            }
        }
    }
}