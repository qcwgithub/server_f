using Data;

namespace Script
{
    public static class MsgTypeExt
    {
        public static bool IsClient(this MsgType type)
        {
            return type >= MsgType.ClientStart;
        }

        // 消息类型是否可并行
        public static bool CanParallel(this MsgType type)
        {
            if (type < MsgType.ClientStart)
            {
                return true;
            }

            switch (type)
            {
                default:
                    return false;
            }
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
                case MsgType.UserLogin:
                    return false;

                default:
                    return true;
            }
        }

        // 玩掉线了要重连，如果之前有消息未处理完，是否需要等处理完了才可以重连
        public static bool DelayLoginIfHandling(this MsgType msgType)
        {
            switch (msgType)
            {
                case MsgType.UserLogin: // 这个一定不能等了。也不需要，因为那时候 socket 还未绑定 player
                    return false;

                default:
                    return true;
            }
        }
    }
}