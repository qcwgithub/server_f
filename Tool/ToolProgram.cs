using Data;

public class ToolProgram
{

    public static void Main(string[] args)
    {
        var argMap = ParseArguments(args);

        string program = TryGetArg(argMap, "program");
        if (program == null)
        {
            AskHelp.AskSelect("which program?", "robot*", "server").OnAnswer((index, answer) =>
            {
                program = answer;
            });
        }

        if (program == "robot")
        {
            new RobotProgram();
        }
        else if (program == "server")
        {
        }
    }
}
