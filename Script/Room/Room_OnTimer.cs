using Data;

namespace Script
{
    [AutoRegister(true)]
    public class Room_OnTimer : OnTimer<RoomService>
    {
        public Room_OnTimer(Server server, RoomService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgTimer msg, ResTimer res)
        {
            switch (msg.timerType)
            {
                case TimerType.SaveRoom:
                    {
                        var save = (TimerSaveRoom)msg.data;

                        Room? room = await this.service.LockRoom(save.roomId, context);
                        if (room == null)
                        {
                            return ECode.Success;
                        }

                        return await this.service.SaveRoom(room, "timer");
                    }

                default:
                    return await base.Handle(context, msg, res);
            }
        }

        public override void PostHandle(MessageContext context, MsgTimer msg, ECode e, ResTimer res)
        {
            switch (msg.timerType)
            {
                case TimerType.SaveRoom:
                    {
                        var save = (TimerSaveRoom)msg.data;
                        this.service.TryUnlockRoom(save.roomId, context);
                    }
                    break;
            }
            base.PostHandle(context, msg, e, res);
        }
    }
}