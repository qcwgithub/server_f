using System;
using MessagePack;

namespace Data
{
    [Flags]
    public enum LogoutFlags
    {
        None = 0,
        LogoutSdk = 1,
        CancelAutoLogin = 2,
    }

    [MessagePackObject]
    public class MsgKick
    {
        [Key(0)]
        public LogoutFlags flags;
    }
}