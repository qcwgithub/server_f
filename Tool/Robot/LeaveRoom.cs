using Data;

namespace Tool
{
    public partial class Robot
    {
        public async Task<ECode> LeaveRoom(long roomId)
        {
            this.Log($"LeaveRoom roomId {roomId}");

            var msg = new MsgLeaveRoom();
            msg.roomId = roomId;

            var r = await this.connection.Request(MsgType.LeaveRoom, msg);
            this.Log($"LeaveRoom result {r.e}");
            if (r.e != ECode.Success)
            {
                return r.e;
            }
            return ECode.Success;
        }
    }
}