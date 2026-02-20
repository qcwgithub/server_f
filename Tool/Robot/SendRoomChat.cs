using Data;

namespace Tool
{
    public partial class Robot
    {
        public async Task<ECode> SendSceneChat(long roomId, string content)
        {
            this.Log($"SendRoomChat roomId {roomId}");

            var msg = new MsgSendSceneChat();
            msg.roomId = roomId;
            msg.chatMessageType = ChatMessageType.Text;
            msg.content = content;

            var r = await this.connection.Request(MsgType.SendSceneChat, msg);
            this.Log($"SendSceneChat result {r.e}");
            if (r.e != ECode.Success)
            {
                return r.e;
            }
            return ECode.Success;
        }
    }
}