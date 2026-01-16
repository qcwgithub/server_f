using Data;

namespace Script
{
    public class Room_LoadRoom : Handler<RoomService, MsgRoomLoadRoom, ResRoomLoadRoom>
    {
        public Room_LoadRoom(Server server, RoomService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Room_LoadRoom;
        public override async Task<ECode> Handle(MessageContext context, MsgRoomLoadRoom msg, ResRoomLoadRoom res)
        {
            Room? room = await this.service.LockRoom(msg.roomId, context);
            if (room == null)
            {
                ECode e;
                (e, room) = await this.service.ss.LoadRoom(msg.roomId);
                if (e != ECode.Success)
                {
                    return e;
                }

                //
                List<ChatMessage> recents = await this.server.roomMessagesRedis.GetRecents(msg.roomId, 100);
                foreach (ChatMessage message in recents)
                {
                    this.service.sd.recentMessages.Enqueue(message);
                }
            }

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgRoomLoadRoom msg, ECode e, ResRoomLoadRoom res)
        {
            this.service.TryUnlockRoom(msg.roomId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}