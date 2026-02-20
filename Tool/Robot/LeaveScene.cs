using Data;

namespace Tool
{
    public partial class Robot
    {
        public async Task<ECode> LeaveScene(long roomId)
        {
            this.Log($"LeaveScene roomId {roomId}");

            var msg = new MsgLeaveScene();
            msg.roomId = roomId;

            var r = await this.connection.Request(MsgType.LeaveScene, msg);
            this.Log($"LeaveScene result {r.e}");
            if (r.e != ECode.Success)
            {
                return r.e;
            }
            return ECode.Success;
        }
    }
}