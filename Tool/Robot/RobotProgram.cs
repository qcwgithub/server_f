using System.Threading.Tasks;
using Data;
using ZstdSharp.Unsafe;

namespace Tool
{
    public partial class RobotProgram
    {
        Dictionary<string, Robot> robotDict = new();
        public async void Start()
        {
            string range = AskHelp.AskInput("channelUserId?").OnAnswer();
            int min;
            int max;

            if (string.IsNullOrEmpty(range))
            {
                min = 1;
                max = 1;
            }
            else
            {
                int index = range.IndexOf('-');
                if (index > 0)
                {
                    min = int.Parse(range.Substring(0, index));
                    max = Math.Max(min, int.Parse(range.Substring(index + 1)));
                }
                else
                {
                    min = int.Parse(range);
                    max = min;
                }
            }

            for (int id = min; id <= max; id++)
            {
                var robot = new Robot(id.ToString());
                this.robotDict.Add(robot.channelUserId, robot);
            }

            var tasks = new List<Task>();
            foreach (var pair in robotDict)
            {
                tasks.Add(pair.Value.Start());
            }

            await Task.WhenAll(tasks);
        }
    }
}