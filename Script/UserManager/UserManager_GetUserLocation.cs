using Data;

namespace Script
{
    public class UserManager_GetUserLocation : Handler<UserManagerService, MsgUserManagerGetUserLocation, ResUserManagerGetUserLocation>
    {
        public UserManager_GetUserLocation(Server server, UserManagerService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._UserManager_GetUserLocation;

        public override async Task<ECode> Handle(MessageContext context, MsgUserManagerGetUserLocation msg, ResUserManagerGetUserLocation res)
        {
            stObjectLocation location = await this.service.userLocator.GetLocation(msg.userId);
            if (location.IsValid())
            {
                res.location = location;
                return ECode.Success;
            }

            if (msg.addWhenNotExist)
            {
                var msgDb = new MsgQuery_AccountInfo_byElementOf_userIds();
                msgDb.ele_userIds = msg.userId;

                var r = await this.service.dbServiceProxy.Query_AccountInfo_byElementOf_userIds(msgDb);
                if (r.e != ECode.Success)
                {
                    return r.e;
                }

                var resDb = r.CastRes<ResQuery_AccountInfo_byElementOf_userIds>();
                if (resDb.result == null)
                {
                    return ECode.AccountNotExist;
                }

                msg.channel = resDb.result.channel;
                msg.channelUserId = resDb.result.channelUserId;

                context.lockValue = await this.server.lockRedis.LockAccount(msg.channel, msg.channelUserId, this.service.logger);
                if (context.lockValue == null)
                {
                    return ECode.RedisLockFail;
                }

                location = await this.service.userLocationAssignmentScript.AssignLocation(msg.userId);
                if (!location.IsValid())
                {
                    return ECode.NoAvailableUserService;
                }

                this.service.userLocator.CacheLocation(msg.userId, location);
            }

            res.location = location;
            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgUserManagerGetUserLocation msg, ECode e, ResUserManagerGetUserLocation res)
        {
            if (context.lockValue != null)
            {
                this.server.lockRedis.UnlockAccount(msg.channel, msg.channelUserId, context.lockValue).Forget(this.service);
            }
        }
    }
}