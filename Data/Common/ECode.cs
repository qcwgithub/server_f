namespace Data
{
    public enum ECode
    {
        Success = 0,
        NoAvailableUserService = 1,
        InvalidAccount = 2,
        InvalidPassword = 3,
        UserNotExist = 5,
        RoomNotExist = 6,
        ServerNotReady = 8,
        Exception = 14,
        InvalidParam = 17,
        Error = 21,
        ServiceIsShuttingDown = 23,
        UserDestroying = 25,
        Room = 26,
        RoomDestroying = 27,
        ServiceConfigError = 31,
        Blocked = 32,
        DelayLogin = 33,
        UserInfoNotExist = 43,
        ServerBusy = 44,
        Server_Timeout = 45,
        Server_NotConnected = 46,
        RoomInfoNotExist = 43,
        InvalidChannel = 85,
        DBErrorAffectedRowCount = 150,

        RedisLockFail = 179,
        MsgProcessing = 227,

        MonitorRunLoop = 228,
        NotEnoughCount = 650,
    }
}