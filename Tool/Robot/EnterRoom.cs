using Data;

namespace Tool
{
    public partial class Robot
    {
        public async Task<ECode> EnterRoom(long roomId)
        {
            this.Log($"EnterRoom roomId {roomId}");

            var msg = new MsgEnterRoom();
            msg.roomId = roomId;

            var r = await this.connection.Request(MsgType.EnterRoom, msg);
            this.Log($"EnterRoom result {r.e}");
            if (r.e != ECode.Success)
            {
                return r.e;
            }
            return ECode.Success;
        }
    }
}