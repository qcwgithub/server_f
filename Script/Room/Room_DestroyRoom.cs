
using System.Collections;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class Room_DestroyRoom : RoomHandler<MsgRoomDestroyRoom, ResRoomDestroyRoom>
    {
        public Room_DestroyRoom(Server server, RoomService service) : base(server, service)
        {
        }


        public override MsgType msgType => MsgType._Room_DestroyRoom;

        protected override async Task<ECode> Handle(ServiceConnection connection, MsgRoomDestroyRoom msg, ResRoomDestroyRoom res)
        {
            var sd = this.service.sd;

            this.service.logger.InfoFormat("{0} roomId {1}, reason {2}, preCount {3}", this.msgType, msg.roomId, msg.reason, sd.roomCount);

            Room? room = sd.GetRoom(msg.roomId);
            if (room == null)
            {
                logger.InfoFormat("{0} room not exist, roomId: {1}", this.msgType, msg.roomId);
                return ECode.RoomNotExist;
            }

            this.service.ss.ClearSaveTimer(room);

            room.destroying = true;

            // 保存一次
            var msgSave = new MsgSaveRoom();
            msgSave.roomId = msg.roomId;
            msgSave.reason = "Room_DestroyRoom";

            var r = await this.service.connectToSelf.Request<MsgSaveRoom, ResSaveRoom>(MsgType._Room_SaveRoom, msgSave);
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            sd.RemoveRoom(msg.roomId);
            this.service.CheckUpdateRuntimeInfo().Forget();
            return ECode.Success;
        }
    }
}