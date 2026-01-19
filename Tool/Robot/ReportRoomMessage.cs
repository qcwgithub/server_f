using Data;

namespace Tool
{
    public partial class Robot
    {
        public async Task<ECode> ReportRoomMessage(long roomId, long messageId)
        {
            this.Log($"ReportRoomMessage roomId {roomId}");

            var msg = new MsgReportRoomMessage();
            msg.roomId = roomId;
            msg.messageId = messageId;
            msg.reason = RoomMessageReportReason.Ads;

            var r = await this.connection.Request(MsgType.ReportRoomMessage, msg);
            this.Log($"ReportRoomMessage result {r.e}");
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            return ECode.Success;
        }
    }
}