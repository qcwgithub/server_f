using System;
using System.Runtime.InteropServices;

namespace Data
{
    public class CtrlCHandler
    {
        // https://learn.microsoft.com/en-us/windows/console/setconsolectrlhandler?WT.mc_id=DT-MVP-5003978
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(SetConsoleCtrlEventHandler handler, bool add);

        // https://learn.microsoft.com/en-us/windows/console/handlerroutine?WT.mc_id=DT-MVP-5003978
        private delegate bool SetConsoleCtrlEventHandler(CtrlType sig);
        static SetConsoleCtrlEventHandler s_handler;

        private enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        static Action s_onExit;
        public static void Register(Action onExit)
        {
            s_onExit = onExit;

            if (s_handler == null)
            {
                s_handler = new SetConsoleCtrlEventHandler(Handler);
            }
            // Register the handler
            SetConsoleCtrlHandler(s_handler, true);
        }

        private static bool Handler(CtrlType signal)
        {
            switch (signal)
            {
                case CtrlType.CTRL_C_EVENT:
                    {
                        s_onExit();
                        return true;
                    }
                case CtrlType.CTRL_BREAK_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                default:
                    return false;
            }
        }
    }
}