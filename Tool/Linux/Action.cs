namespace Tool
{
    public enum Action
    {
        ViewServices,
        ShutdownServices,
        ShutdownServicesAll,
        PrintUserUSId,
        PrintPendingMsgList,
        UserAction,
        ShowScriptVersion,
        ReloadScript,
        ReloadConfigs,
        GetReloadConfigOptions,
        RevokeChat,
        USAction,
        TaskQueueLengthes,
        ImportRoomConfig,
        SearchRoom,
        Exit,

        Count,
    }

    public static class ActionExt
    {
        public static string ToText(this Action action)
        {
            switch (action)
            {
                case Action.ViewServices:
                    return "View Services";
                case Action.ShutdownServices:
                    return "Shutdown Services";
                case Action.ShutdownServicesAll:
                    return "Shutdown Services All";
                case Action.PrintUserUSId:
                    return "Print User US Id";
                case Action.PrintPendingMsgList:
                    return "Print Pending Msg List";
                case Action.UserAction:
                    return "User Action";
                case Action.ShowScriptVersion:
                    return "Show Script Version";
                case Action.ReloadScript:
                    return "Reload Script";
                case Action.ReloadConfigs:
                    return "Reload Configs";
                case Action.GetReloadConfigOptions:
                    return "Get Reload Config Options";
                case Action.RevokeChat:
                    return "Revoke Chat";
                case Action.USAction:
                    return "US Action";
                case Action.TaskQueueLengthes:
                    return "Task Queue Lengthes";
                case Action.ImportRoomConfig:
                    return "Import Room Config";
                case Action.SearchRoom:
                    return "Search Room";
                case Action.Exit:
                    return "Exit";
                default:
                    return string.Empty;
            }
        }

        static List<Action>? _allActions;
        public static List<Action> allActions
        {
            get
            {
                if (_allActions == null)
                {
                    _allActions = new List<Action>();
                    for (Action e = 0; e < Action.Count; e++)
                    {
                        _allActions.Add(e);
                    }
                }
                return _allActions;
            }
        }
    }
}