using Data;

namespace Script
{
    public class Room_UserEnter : Handler<RoomService, MsgRoomUserEnter, ResRoomUserEnter>
    {
        public Room_UserEnter(Server server, RoomService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Room_UserEnter;
        public override async Task<ECode> Handle(MessageContext context, MsgRoomUserEnter msg, ResRoomUserEnter res)
        {
            this.service.logger.Info($"{this.msgType} userId {msg.userId} roomId {msg.roomId} gatewayServiceId {msg.gatewayServiceId}");

            Room? room = await this.service.LockRoom(msg.roomId, context);
            if (room == null)
            {
                ECode e;
                (e, room) = await this.service.ss.LoadRoom(msg.roomId);
                if (e != ECode.Success)
                {
                    return e;
                }
            }

            RoomUser? user = room.GetUser(msg.userId);
            if (user == null)
            {
                user = new RoomUser();
                user.userId = msg.userId;
                room.AddUser(user);
            }

            user.gatewayServiceId = msg.gatewayServiceId;
            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgRoomUserEnter msg, ECode e, ResRoomUserEnter res)
        {
            this.service.TryUnlockRoom(msg.roomId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}