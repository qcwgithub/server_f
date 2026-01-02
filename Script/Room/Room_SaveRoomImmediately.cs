using System.Threading.Tasks;
using Data;
namespace Script
{
    // 其他服 -> Room
    public class Room_SaveRoomImmediately : RoomHandler<MsgSaveRoomImmediately, ResSaveRoomImmediately>
    {
        public Room_SaveRoomImmediately(Server server, RoomService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Room_SaveRoomImmediately;

        public override async Task<ECode> Handle(MessageContext context, MsgSaveRoomImmediately msg, ResSaveRoomImmediately res)
        {
            ECode e = await this.service.SaveRoom(msg.roomId, msg.reason);
            return e;
        }
    }
}