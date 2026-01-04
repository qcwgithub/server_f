using Data;

namespace Script
{
    public class User_OnTimer : OnTimer<UserService>
    {
        public User_OnTimer(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgTimer msg, ResTimer res)
        {
            switch (msg.timerType)
            {
                case TimerType.SaveUser:
                    {
                        var save = (TimerSaveUser)msg.data;

                        User? user = await this.service.LockUser(save.userId, context);
                        if (user == null)
                        {
                            return ECode.Success;
                        }

                        return await this.service.SaveUser(user, "timer");
                    }

                default:
                    return await base.Handle(context, msg, res);
            }
        }

        public override void PostHandle(MessageContext context, MsgTimer msg, ECode e, ResTimer res)
        {
            switch (msg.timerType)
            {
                case TimerType.SaveUser:
                    {
                        var save = (TimerSaveUser)msg.data;
                        this.service.TryUnlockUser(save.userId, context);
                    }
                    break;
            }
            base.PostHandle(context, msg, e, res);
        }
    }
}