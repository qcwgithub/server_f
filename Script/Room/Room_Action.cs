using Data;

namespace Script
{
    [AutoRegister]
    public class Room_Action : Handler<RoomService, MsgRoomServiceAction, ResRoomServiceAction>
    {
        public Room_Action(Server server, RoomService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Room_ServerAction;

        public override async Task<ECode> Handle(MessageContext context, MsgRoomServiceAction msg, ResRoomServiceAction res)
        {
            this.service.logger.Info(this.msgType);
            var sd = this.service.sd;

            if (msg.allowNewRoom != null)
            {
                bool pre = sd.allowNewRoom;
                bool curr = msg.allowNewRoom.Value;

                this.service.logger.InfoFormat("allowNewRoom {0} -> {1}", pre, curr);

                if (pre != curr)
                {
                    sd.allowNewRoom = curr;
                }
            }

            if (msg.saveIntervalS != null)
            {
                int pre = sd.saveIntervalS;
                int curr = msg.saveIntervalS.Value;

                this.service.logger.InfoFormat("saveIntervalS {0} -> {1}", pre, curr);

                if (pre != curr)
                {
                    sd.saveIntervalS = curr;
                }
            }

            return ECode.Success;
        }
    }
}