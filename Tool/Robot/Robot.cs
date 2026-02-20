using Data;

namespace Tool
{
    public partial class Robot
    {
        public readonly string channelUserId;
        public Robot(string channelUserId)
        {
            this.channelUserId = channelUserId;
        }

        void Log(string format, params object?[] args)
        {
            Console.WriteLine("{0} {1}", this.channelUserId, string.Format(format, args));
        }

        void LogEx(ConsoleColor color, string format, params object?[] args)
        {
            ConsoleEx.WriteLine(color, string.Format("{0} {1}", this.channelUserId, string.Format(format, args)));
        }

        ToolConnectionCallback connectionCallback;
        ToolConnection connection;
        ResLogin resLogin;
        ResGetRecommendedScenes resGetRecommendedScenes;
        long roomId;
        public async Task Start()
        {
            this.connectionCallback = new ToolConnectionCallback();
            var tcsConnect = new TaskCompletionSource<bool>();
            this.connectionCallback.onConnect = success => tcsConnect.SetResult(success);

            this.connectionCallback.onMsg = (msgType, msg, reply) =>
            {
                switch (msgType)
                {
                    case MsgType.AChatMessage:
                        {
                            var chatMsg = ((MsgAChatMessage)msg).message;
                            Console.WriteLine($"Received chat message from {chatMsg.senderId}: {chatMsg.content} reply == null? {reply == null}");
                        }
                        break;
                }
            };

            this.connection = new ToolConnection(this.connectionCallback, "localhost", 8020);

            this.connection.Connect();
            bool success = await tcsConnect.Task;
            Console.WriteLine($"Connect result {success}");
            if (!success)
            {
                Console.ReadLine();
                return;
            }

            ECode e;
            do
            {
                (e, this.resLogin) = await this.Login();
                if (e != ECode.Success)
                {
                    Console.ReadLine();
                    break;
                }

                (e, this.resGetRecommendedScenes) = await this.GetRecommendedScenes();
                if (e != ECode.Success)
                {
                    Console.ReadLine();
                    break;
                }

                if (this.resGetRecommendedScenes.sceneInfos.Count == 0)
                {
                    Console.ReadLine();
                    break;
                }

                ResEnterRoom resEnterRoom;
                (e, resEnterRoom) = await this.EnterScene(this.resGetRecommendedScenes.sceneInfos[0].sceneId);
                if (e != ECode.Success)
                {
                    Console.ReadLine();
                    break;
                }

                this.roomId = this.resGetRecommendedScenes.sceneInfos[0].sceneId;

                if (resEnterRoom.recentMessages.Count > 0)
                {
                    e = await this.ReportSceneMessage(this.roomId, resEnterRoom.recentMessages[0].messageId);
                    if (e != ECode.Success)
                    {
                        Console.ReadLine();
                        break;
                    }
                }

                for (int i = 0; i < 100; i++)
                {
                    e = await this.SendSceneChat(this.roomId, $"Hello {DateTime.Now}!");
                    if (e != ECode.Success)
                    {
                        Console.ReadLine();
                        break;
                    }
                    await Task.Delay(2000);
                }

                // e = await this.LeaveRoom(this.roomId);
                // if (e != ECode.Success)
                // {
                //     Console.ReadLine();
                //     break;
                // }

                // (e, resEnterRoom) = await this.EnterRoom(this.roomId, lastMessageId);
                // if (e != ECode.Success)
                // {
                //     Console.ReadLine();
                //     break;
                // }

                e = await this.SetName("Test-Name");
                if (e != ECode.Success)
                {
                    Console.ReadLine();
                    break;
                }

                while (true)
                {
                    await Task.Delay(1000);
                }
            }
            while (false);

            this.connection.Close("Doesn't matter");
        }
    }
}