using Data;

namespace Tool
{
    public partial class Robot
    {
        public async Task<ECode> EnterRoom(long roomId)
        {
            Console.WriteLine($"EnterRoom roomId {roomId}");

            var msgEnter = new MsgEnterRoom();
            msgEnter.roomId = roomId;

            var r = await this.connection.Request(MsgType.EnterRoom, msgEnter);
            Console.WriteLine($"EnterRoom result {r.e}");
            if (r.e != ECode.Success)
            {
                return r.e;
            }
            return ECode.Success;
        }
    }
}