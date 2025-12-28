using System.Threading.Tasks;
using Data;
namespace Script
{
    // 其他服 -> Room
    public class Room_SaveRoomImmediately : RoomHandler<MsgSaveRoom, ResSaveRoom>
    {
        public Room_SaveRoomImmediately(Server server, RoomService service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Room_SaveRoomImmediately;

        protected override async Task<ECode> Handle(ServiceConnection connection, MsgSaveRoom msg, ResSaveRoom res)
        {
            var r = await this.service.connectToSelf.Request<MsgSaveRoom, ResSaveRoom>(MsgType._Room_SaveRoom, msg);
            return r.e;
        }
    }
}