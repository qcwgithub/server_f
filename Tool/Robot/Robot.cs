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

        ToolConnection connection;
        ResLogin resLogin;
        ResGetRecommendedRooms resGetRecommendedRooms;
        long roomId;
        public async Task Start()
        {
            this.connection = new ToolConnection("localhost", 8020);

            bool success = await this.connection.Connect();
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

            this.connection.Close();
        }
    }
}