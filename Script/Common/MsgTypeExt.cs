using Data;

namespace Script
{
    public static class MsgTypeExt
    {
        public static bool IsClient(this MsgType type)
        {
            return type >= MsgType.ClientStart;
        }

        public static bool LogErrorIfNotSuccess(this MsgType msgType)
        {
            if (msgType < MsgType.ClientStart)
            {
                // 服务器单独处理
                return false;
            }

            switch (msgType)
            {
                case MsgType.Login:
                    return false;

                default:
                    return true;
            }
        }
    }
}