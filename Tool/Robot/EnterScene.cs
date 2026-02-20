using Data;

namespace Tool
{
    public partial class Robot
    {
        public async Task<(ECode, ResEnterRoom)> EnterScene(long roomId)
        {
            this.Log($"EnterRoom roomId {roomId}");

            var msg = new MsgEnterScene();
            msg.roomId = roomId;
            msg.lastMessageId = 0;

            var r = await this.connection.Request(MsgType.EnterScene, msg);
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
                    var m = res.recentMessages[i];
                    Console.WriteLine($"recent[{i}] messageId: {m.messageId} {TimeUtils.MillisecondsToDateTime(m.timestamp)} senderId: {m.senderId} content: {m.content}");
                }

                if (res.recentMessages.Count > 0)
                {
                    long lastMessageId = res.recentMessages[0].messageId;
                    while (lastMessageId > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Requesting scene chat history..., lastMessageId {0}", lastMessageId);

                        var msgHistory = new MsgGetSceneChatHistory
                        {
                            roomId = roomId,
                            lastMessageId = lastMessageId,
                        };

                        r = await this.connection.Request(MsgType.GetSceneChatHistory, msgHistory);

                        var resHistory = r.CastRes<ResGetSceneChatHistory>();
                        Console.WriteLine("history count: {0}", resHistory.history.Count);
                        for (int i = 0; i < resHistory.history.Count; i++)
                        {
                            var m = resHistory.history[i];
                            Console.WriteLine($"history[{i}] messageId: {m.messageId} {TimeUtils.MillisecondsToDateTime(m.timestamp)} senderId: {m.senderId} content: {m.content}");
                        }

                        if (resHistory.history.Count > 0)
                        {
                            lastMessageId = resHistory.history[0].messageId;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return (ECode.Success, res);
        }
    }
}