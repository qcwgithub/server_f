using Data;

namespace Tool
{
    public partial class Robot
    {
        public async Task<(ECode, ResGetRecommendedScenes)> GetRecommendedScenes()
        {
            this.Log("Get Recommended Rooms");

            var msg = new MsgGetRecommendedScenes();

            var r = await this.connection.Request(MsgType.GetRecommendedScenes, msg);
            this.Log($"GetRecommendedRooms {r.e}");
            if (r.e != ECode.Success)
            {
                return (r.e, null);
            }

            var res = r.CastRes<ResGetRecommendedScenes>();

            return (ECode.Success, res);
        }
    }
}