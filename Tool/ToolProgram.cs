using Data;

namespace Tool
{
    public class ToolProgram
    {
        public static void Main(string[] args)
        {
            new ToolProgram(args);
        }

        public static ToolProgram Instance;
        public readonly ArgMap argMap;
        public ToolProgram(string[] args)
        {
            Instance = this;
            ET.ThreadSynchronizationContext.CreateInstance();
            SynchronizationContext.SetSynchronizationContext(ET.ThreadSynchronizationContext.Instance);

            this.argMap = ArgMap.ParseArguments(args);

            string? program = this.argMap.GetArg("program");
            if (program == null)
            {
                (_, program) = AskHelp.AskSelect("which program?", "robot*", "server", "linux").OnAnswer2(); 
            }

            if (program == "robot")
            {
                new RobotProgram().Start();
            }
            else if (program == "server")
            {
            }
            else if (program == "linux")
            {
                new LinuxProgram().Start();
            }

            while (true)
            {
                Thread.Sleep(1);
                ET.ThreadSynchronizationContext.Instance.Update();
            }
        }
    }
}