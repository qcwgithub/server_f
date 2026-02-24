using Data;

namespace Script
{
    [AutoRegister]
    public class RoomManager_CreateFriendChatRoom : Handler<RoomManagerService, MsgRoomManagerCreateFriendChatRoom, ResRoomManagerCreateFriendChatRoom>
    {
        public RoomManager_CreateFriendChatRoom(Server server, RoomManagerService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._RoomManager_CreateFriendChatRoom;

        public override async Task<ECode> Handle(MessageContext context, MsgRoomManagerCreateFriendChatRoom msg, ResRoomManagerCreateFriendChatRoom res)
        {
            this.service.logger.Info($"{this.msgType} participants {JsonUtils.stringify(msg.userIds)}");

            if (msg.userIds == null || msg.userIds.Count != 2)
            {
                return ECode.InvalidParam;
            }

            for (int i = 0; i < msg.userIds.Count; i++)
            {
                long userId = msg.userIds[i];
                for (int j = i + 1; j < msg.userIds.Count; j++)
                {
                    if (userId == msg.userIds[j])
                    {
                        return ECode.Duplicate;
                    }
                }
            }

            long roomId = this.service.roomIdSnowflakeScript.NextRoomId();
            FriendChatInfo privateRoomInfo = this.service.roomScript.NewPrivateRoomInfo(roomId);

            long nowS = TimeUtils.GetTimeS();
            foreach (long userId in msg.userIds)
            {
                var roomUser = PrivateRoomUser.Ensure(null);
                roomUser.userId = userId;
                roomUser.joinTimeS = nowS;
                privateRoomInfo.users.Add(roomUser);
            }

            ECode e = await this.service.roomScript.InsertPrivateSceneInfo(privateRoomInfo);
            if (e != ECode.Success)
            {
                return e;
            }

            res.friendChatInfo = privateRoomInfo;
            return ECode.Success;
        }
    }
}