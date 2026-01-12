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

            this.connection = new ToolConnection(this.connectionCallback, "localhost", 8020);

            this.connection.Connect();
            bool success = await tcsConnect.Task;
            Console.WriteLine($"Connect result {success}");
            if (!success)
            {
                return;
            }

            ECode e;
            do
            {
                (e, this.resLogin) = await this.Login();
                if (e != ECode.Success)
                {
                    break;
                }

                (e, this.resGetRecommendedRooms) = await this.GetRecommendedRooms();
                if (e != ECode.Success)
                {
                    break;
                }

                if (this.resGetRecommendedRooms.roomInfos.Count == 0)
                {
                    break;
                }

                e = await this.EnterRoom(this.resGetRecommendedRooms.roomInfos[0].roomId);
                if (e != ECode.Success)
                {
                    break;
                }

                this.roomId = this.resGetRecommendedRooms.roomInfos[0].roomId;

                // e = await this.LeaveRoom(this.roomId);
                // if (e != ECode.Success)
                // {
                //     break;
                // }

                e = await this.SendRoomChat(this.roomId, "Hello!");
                if (e != ECode.Success)
                {
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