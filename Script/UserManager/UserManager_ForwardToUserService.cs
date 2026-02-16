using Data;

namespace Script
{
    public class UserManager_ForwardToUserService : Handler<UserManagerService, MsgForwardToUserService, ResForwardToUserService>
    {
        public UserManager_ForwardToUserService(Server server, UserManagerService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._UserManager_ForwardToUserService;

        public override async Task<ECode> Handle(MessageContext context, MsgForwardToUserService msg, ResForwardToUserService res)
        {
            stObjectLocation location = await this.service.userLocator.GetLocation(msg.userId);
            if (!location.IsValid())
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

            var msgUser = MessageTypeConfigData.DeserializeMsg(msg.innerMsgType, msg.innerMsgBytes);
            var r = await  this.service.userServiceProxy.Request(location.serviceId, msg.innerMsgType, msgUser);
            res.innerResBytes = MessageTypeConfigData.SerializeRes(msg.innerMsgType, r.res);

            return r.e;
        }

        public override void PostHandle(MessageContext context, MsgForwardToUserService msg, ECode e, ResForwardToUserService res)
        {
            if (context.lockValue != null)
            {
                this.server.lockRedis.UnlockAccount(msg.channel, msg.channelUserId, context.lockValue).Forget(this.service);
            }
        }
    }
}