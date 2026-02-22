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
            msg.lastSeq = 0;

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
                    Console.WriteLine($"recent[{i}] seq: {m.seq} {TimeUtils.MillisecondsToDateTime(m.timestamp)} senderId: {m.senderId} content: {m.content}");
                }

                if (res.recentMessages.Count > 0)
                {
                    long lastSeq = res.recentMessages[0].seq;
                    while (lastSeq > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Requesting scene chat history..., lastSeq {0}", lastSeq);

                        var msgHistory = new MsgGetSceneChatHistory
                        {
                            roomId = roomId,
                            lastSeq = lastSeq,
                        };

                        r = await this.connection.Request(MsgType.GetSceneChatHistory, msgHistory);

                        var resHistory = r.CastRes<ResGetSceneChatHistory>();
                        Console.WriteLine("history count: {0}", resHistory.history.Count);
                        for (int i = 0; i < resHistory.history.Count; i++)
                        {
                            var m = resHistory.history[i];
                            Console.WriteLine($"history[{i}] seq: {m.seq} {TimeUtils.MillisecondsToDateTime(m.timestamp)} senderId: {m.senderId} content: {m.content}");
                        }

                        if (resHistory.history.Count > 0)
                        {
                            lastSeq = resHistory.history[0].seq;
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