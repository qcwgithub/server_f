using Data;

namespace Script
{
    public class Room_SendChat : RoomHandler<MsgRoomSendChat, ResRoomSendChat>
    {
        public Room_SendChat(Server server, RoomService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Room_SendChat;

        public override async Task<ECode> Handle(MessageContext context, MsgRoomSendChat msg, ResRoomSendChat res)
        {
            Room? room = await this.service.LockRoom(msg.roomId, context);
            if (room == null)
            {
                return ECode.RoomNotExist;
            }

            RoomUser? user = room.GetUser(msg.userId);
            if (user == null)
            {
                return ECode.UserNotInRoom;
            }

            var broadcast = new A_MsgRoomChat();

            Dictionary<int, List<RoomUser>> dict = room.userDict
                .GroupBy(pair => pair.Value.gatewayServiceId, pair => pair.Value)
                .ToDictionary(group => group.Key, group => group.ToList());

            foreach (var pair in dict)
            {
                int gatewayServiceId = pair.Key;
                List<RoomUser> roomUsers = pair.Value;

                List<long> userIds = roomUsers.Select(x => x.userId).ToList();
                ECode e = this.service.gatewayServiceProxy.BroadcastToClient(gatewayServiceId, userIds, MsgType.A_RoomChat, broadcast);
                if (e == ECode.Server_NotConnected)
                {
                    this.service.logger.Warn($"{this.msgType} gatewayServiceId {gatewayServiceId} is not connected");
                }
            }

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgRoomSendChat msg, ECode e, ResRoomSendChat res)
        {
            this.service.TryUnlockRoom(msg.roomId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}