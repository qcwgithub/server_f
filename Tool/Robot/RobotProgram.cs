using System.Threading.Tasks;
using Data;
using ZstdSharp.Unsafe;

namespace Tool
{
    public partial class RobotProgram
    {
        Dictionary<string, Robot> robotDict = new();
        public async Task Start()
        {
            for (int i = 0; i < 1; i++)
            {
                var robot = new Robot((1000 + i).ToString());
                this.robotDict.Add(robot.channelUserId, robot);
            }

            foreach (var pair in robotDict)
            {
                await pair.Value.Start();
            }
        }
    }
}