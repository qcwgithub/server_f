using Data;

namespace Tool
{
    public partial class Robot
    {
        public async Task<(ECode, ResGetRecommendedRooms)> GetRecommendedRooms()
        {
            Console.WriteLine($"Get Recommended Rooms");

            var msg = new MsgGetRecommendedRooms();

            var r = await this.connection.Request(MsgType.GetRecommendedRooms, msg);
            Console.WriteLine($"GetRecommendedRooms {r.e}");
            if (r.e != ECode.Success)
            {
                return (r.e, null);
            }

            var res = r.CastRes<ResGetRecommendedRooms>();

            return (ECode.Success, res);
        }
    }
}