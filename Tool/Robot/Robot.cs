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
        ResGetRecommendedRooms resGetRecommendedRooms;
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
                    case MsgType.A_RoomChat:
                        var chatMsg = (ChatMessage)msg;
                        Console.WriteLine($"Received chat message from {chatMsg.senderId}: {chatMsg.content} reply == null? {reply == null}");
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

                (e, this.resGetRecommendedRooms) = await this.GetRecommendedRooms();
                if (e != ECode.Success)
                {
                    Console.ReadLine();
                    break;
                }

                if (this.resGetRecommendedRooms.roomInfos.Count == 0)
                {
                    Console.ReadLine();
                    break;
                }

                long lastMessageId = 0;

                ResEnterRoom resEnterRoom;
                (e, resEnterRoom) = await this.EnterRoom(this.resGetRecommendedRooms.roomInfos[0].roomId, lastMessageId);
                if (e != ECode.Success)
                {
                    Console.ReadLine();
                    break;
                }

                this.roomId = this.resGetRecommendedRooms.roomInfos[0].roomId;

                if (resEnterRoom.recentMessages.Count > 0)
                {
                    e = await this.ReportRoomMessage(this.roomId, resEnterRoom.recentMessages[0].messageId);
                    if (e != ECode.Success)
                    {
                        Console.ReadLine();
                        break;
                    }
                }

                // e = await this.SendRoomChat(this.roomId, $"Hello {DateTime.Now}!");
                // if (e != ECode.Success)
                // {
                //     Console.ReadLine();
                //     break;
                // }

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

                // e = await this.SetName("Test-Name");
                // if (e != ECode.Success)
                // {
                //     Console.ReadLine();
                //     break;
                // }

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