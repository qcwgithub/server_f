using Data;

namespace Script
{
    [AutoRegister]
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
                var ret = await this.service.ss.CheckUserExistAndAddLocation(context, msg.userId);
                if (ret.e != ECode.Success)
                {
                    return ret.e;
                }

                msg.channel = ret.channel;
                msg.channelUserId = ret.channelUserId;
                location = ret.location;
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