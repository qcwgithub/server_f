using Data;

namespace Tool
{
    public partial class Robot
    {
        public async Task<(ECode, ResEnterRoom)> EnterRoom(long roomId, long lastMessageId)
        {
            this.Log($"EnterRoom roomId {roomId}");

            var msg = new MsgEnterRoom();
            msg.roomId = roomId;
            msg.lastMessageId = lastMessageId;

            var r = await this.connection.Request(MsgType.EnterRoom, msg);
            this.Log($"EnterRoom result {r.e}");
            if (r.e != ECode.Success)
            {
                return (r.e, null);
            }

            var res = r.CastRes<ResEnterRoom>();
            if (res.recentMessages == null)
            {
                Console.WriteLine("res.recentMessages == null");
            }
            else
            {
                Console.WriteLine($"Recent message count {res.recentMessages.Count}");

                for (int i = 0; i < res.recentMessages.Count; i++)
                {
                    Console.WriteLine($"[{i}] {res.recentMessages[i].senderId} {res.recentMessages[i].content}");
                }
            }

            return (ECode.Success, res);
        }
    }
}