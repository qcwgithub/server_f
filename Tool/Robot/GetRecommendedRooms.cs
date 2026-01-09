using Data;

namespace Tool
{
    public partial class Robot
    {
        public async Task<(ECode, ResGetRecommendedRooms)> GetRecommendedRooms()
        {
            this.Log("Get Recommended Rooms");

            var msg = new MsgGetRecommendedRooms();

            var r = await this.connection.Request(MsgType.GetRecommendedRooms, msg);
            this.Log($"GetRecommendedRooms {r.e}");
            if (r.e != ECode.Success)
            {
                return (r.e, null);
            }

            var res = r.CastRes<ResGetRecommendedRooms>();

            return (ECode.Success, res);
        }
    }
}