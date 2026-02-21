using Data;

namespace Script
{
    [AutoRegister]
    public class Room_UserLeaveScene : Handler<RoomService, MsgRoomUserLeaveScene, ResRoomUserLeaveScene>
    {
        public Room_UserLeaveScene(Server server, RoomService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Room_UserLeaveScene;

        public override async Task<ECode> Handle(MessageContext context, MsgRoomUserLeaveScene msg, ResRoomUserLeaveScene res)
        {
            var sceneRoom = await this.service.LockRoom<SceneRoom>(msg.roomId, context);
            if (sceneRoom == null)
            {
                this.service.logger.ErrorFormat("{0} roomId {1} roomId {2}, room == null!", this.msgType, msg.roomId, msg.roomId);
                return ECode.RoomNotExist;
            }

            sceneRoom.RemoveUser(msg.userId);

            return ECode.Success;
        }

        public override void PostHandle(MessageContext context, MsgRoomUserLeaveScene msg, ECode e, ResRoomUserLeaveScene res)
        {
            this.service.TryUnlockRoom(msg.roomId, context);

            base.PostHandle(context, msg, e, res);
        }
    }
}