using Data;

namespace Script
{
    public class Room_Action : RoomHandler<MsgRoomServiceAction, ResRoomServiceAction>
    {
        public Room_Action(Server server, RoomService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._ServerAction;

        public override async Task<ECode> Handle(IConnection connection, MsgRoomServiceAction msg, ResRoomServiceAction res)
        {
            this.logger.Info(this.msgType);
            var sd = this.service.sd;

            if (msg.allowNewRoom != null)
            {
                bool pre = sd.allowNewRoom;
                bool curr = msg.allowNewRoom.Value;

                this.logger.InfoFormat("allowNewRoom {0} -> {1}", pre, curr);

                if (pre != curr)
                {
                    sd.allowNewRoom = curr;
                }
            }

            if (msg.saveIntervalS != null)
            {
                int pre = sd.saveIntervalS;
                int curr = msg.saveIntervalS.Value;

                this.logger.InfoFormat("saveIntervalS {0} -> {1}", pre, curr);

                if (pre != curr)
                {
                    sd.saveIntervalS = curr;
                }
            }

            return ECode.Success;
        }
    }
}