using System.Threading.Tasks;
using Data;

namespace Tool
{
    public class RobotProgram
    {
        public async void Start()
        {
            var connection = new ToolConnection();

            ////////

            bool success = await connection.Connect("localhost", 8020);
            Console.WriteLine($"Connect result {success}");
            if (!success)
            {
                return;
            }

            ////////

            var msgLogin = new MsgLogin();
            msgLogin.version = string.Empty;
            msgLogin.platform = "Windows";
            msgLogin.channel = MyChannels.uuid;
            msgLogin.channelUserId = "1000";

            Console.WriteLine($"Login channelUserId {msgLogin.channelUserId}");

            var r = await connection.Request(MsgType.Login, msgLogin);
            Console.WriteLine($"Login result {r.e}");
            if (r.e != ECode.Success)
            {
                connection.Close();
                return;
            }

            var resLogin = r.CastRes<ResLogin>();
            Console.WriteLine($"isNewUser? {resLogin.isNewUser} userId {resLogin.userInfo.userId} kickOther? {resLogin.kickOther}");

            ////////

            long roomId = 1;
            Console.WriteLine($"EnterRoom roomId {roomId}");

            var msgEnter = new MsgEnterRoom();
            msgEnter.roomId = roomId;

            r = await connection.Request(MsgType.EnterRoom, msgEnter);
            Console.WriteLine($"EnterRoom result {r.e}");
            if (r.e != ECode.Success)
            {
                connection.Close();
                return;
            }

            ////////

            connection.Close();
        }
    }
}