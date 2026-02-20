using Data;

namespace Tool
{
    public partial class Robot
    {
        public async Task<ECode> ReportSceneMessage(long roomId, long messageId)
        {
            this.Log($"ReportSceneMessage roomId {roomId}");

            var msg = new MsgReportMessage();
            msg.roomId = roomId;
            msg.messageId = messageId;
            msg.reason = MessageReportReason.Ads;

            var r = await this.connection.Request(MsgType.ReportMessage, msg);
            this.Log($"ReportMessage result {r.e}");
            if (r.e != ECode.Success)
            {
                return r.e;
            }

            return ECode.Success;
        }
    }
}