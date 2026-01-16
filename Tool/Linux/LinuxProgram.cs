using Data;

namespace Tool
{
    public partial class LinuxProgram
    {
        Action AskAction()
        {
            Action action = default;
            string? actionStr = ToolProgram.Instance.argMap.GetArg("action");
            if (actionStr == null || !Enum.TryParse<Action>(actionStr, out action))
            {
                (int index, string answer) = AskHelp.AskSelect("What to do?", ActionExt.allActions.Select(x => x.ToText()).ToArray())
                    .OnAnswer2();

                action = ActionExt.allActions[index];
            }

            return action;
        }

        ConfigLoader configLoader = new();
        List<ServiceConfig> allServiceConfigs;
        public async void Start()
        {
            if (!this.configLoader.LoadAllServiceConfigs(out this.allServiceConfigs, out string message))
            {
                ConsoleEx.WriteLine(ConsoleColor.Red, message);
                return;
            }

            bool exit = false;
            while (!exit)
            {
                Action action = this.AskAction();
                switch (action)
                {
                    case Action.ViewServices:
                        this.ViewServices();
                        break;

                    case Action.ShutdownServices:
                    case Action.ShutdownServicesAll:
                        await this.ShutdownServices(action == Action.ShutdownServicesAll);
                        break;

                    case Action.ImportRoomConfig:
                        await this.ImportRoomConfig();
                        break;

                    case Action.SearchRoom:
                        await this.SearchRoom();
                        break;

                    case Action.Exit:
                        exit = true;
                        break;
                }
            }
        }
    }
}