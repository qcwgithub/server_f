using Data;

namespace Script
{
    public class _User_OnTimer : OnTimer<UserService>
    {
        public _User_OnTimer(Server server, UserService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgTimer msg, ResTimer res)
        {
            switch (msg.timerType)
            {
                case TimerType.SaveUser:
                    {
                        var save = msg.data as TimerSaveUser;
                        if (save == null)
                        {
                            throw new Exception("TimerSaveUser data is null");
                        }

                        User? user = await this.service.LockUser(save.userId, context);
                        if (user == null)
                        {
                            return ECode.Success;
                        }

                        return await this.service.SaveUser(user, "timer");
                    }

                case TimerType.DestroyUser:
                    {
                        var destroy = msg.data as TimerDestroyUser;
                        if (destroy == null)
                        {
                            throw new Exception("TimerDestroyUser data is null");
                        }

                        User? user = await this.service.LockUser(destroy.userId, context);
                        if (user == null)
                        {
                            return ECode.Success;
                        }

                        return await this.service.DestroyUser(user, destroy.reason);
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
                        var save = msg.data as TimerSaveUser;
                        if (save == null)
                        {
                            throw new Exception("TimerSaveUser data is null");
                        }
                        this.service.TryUnlockUser(save.userId, context);
                    }
                    break;

                case TimerType.DestroyUser:
                    {
                        var destroy = msg.data as TimerDestroyUser;
                        if (destroy == null)
                        {
                            throw new Exception("TimerDestroyUser data is null");
                        }
                        this.service.TryUnlockUser(destroy.userId, context);
                    }
                    break;
            }
            base.PostHandle(context, msg, e, res);
        }
    }
}