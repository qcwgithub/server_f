using Data;

namespace Tool
{
    public partial class Robot
    {
        public async Task<ECode> SendRoomChat(long roomId, string content)
        {
            this.Log($"SendRoomChat roomId {roomId}");

            var msg = new MsgSendRoomChat();
            msg.roomId = roomId;
            msg.chatMessageType = ChatMessageType.Text;
            msg.content = content;

            var r = await this.connection.Request(MsgType.SendRoomChat, msg);
            this.Log($"SendRoomChat result {r.e}");
            if (r.e != ECode.Success)
            {
                return r.e;
            }
            return ECode.Success;
        }
    }
}