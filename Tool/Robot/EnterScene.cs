using Data;

namespace Tool
{
    public partial class Robot
    {
        public async Task<(ECode, ResEnterScene)> EnterScene(long roomId)
        {
            this.Log($"EnterScene roomId {roomId}");

            var msg = new MsgEnterScene();
            msg.roomId = roomId;
            msg.lastSeq = 0;

            var r = await this.connection.Request(MsgType.EnterScene, msg);
            this.Log($"EnterScene result {r.e}");
            if (r.e != ECode.Success)
            {
                return (r.e, null);
            }

            var res = r.CastRes<ResEnterScene>();
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
                    long beforeSeq = res.recentMessages[0].seq;
                    while (beforeSeq > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Requesting scene chat history..., lastSeq {0}", beforeSeq);

                        var msgHistory = new MsgGetSceneChatHistory
                        {
                            roomId = roomId,
                            beforeSeq = beforeSeq,
                        };

                        r = await this.connection.Request(MsgType.GetSceneChatHistory, msgHistory);

                        var resHistory = r.CastRes<ResGetSceneChatHistory>();
                        Console.WriteLine("history count: {0}", resHistory.messages.Count);
                        for (int i = 0; i < resHistory.messages.Count; i++)
                        {
                            var m = resHistory.messages[i];
                            Console.WriteLine($"history[{i}] seq: {m.seq} {TimeUtils.MillisecondsToDateTime(m.timestamp)} senderId: {m.senderId} content: {m.content}");
                        }

                        if (resHistory.messages.Count > 0)
                        {
                            beforeSeq = resHistory.messages[0].seq;
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