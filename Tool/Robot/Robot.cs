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

        ToolConnection connection;
        ResLogin resLogin;
        ResGetRecommendedRooms resGetRecommendedRooms;
        long roomId;
        public async Task Start()
        {
            this.connection = new ToolConnection();

            bool success = await this.connection.Connect("localhost", 8020);
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

                if (this.resGetRecommendedRooms.roomInfos.Count > 0)
                {
                    e = await this.EnterRoom(this.resGetRecommendedRooms.roomInfos[0].roomId);
                    if (e != ECode.Success)
                    {
                        break;
                    }

                    this.roomId = this.resGetRecommendedRooms.roomInfos[0].roomId;
                }
            }
            while (false);

            this.connection.Close();
        }
    }
}