namespace Data
{
    public enum TimerType
    {
        // Shared
        Shutdown,
        CheckConnections,

        // User
        SaveUser,
        DestroyUser,

        // Room
        SaveRoom,
        DestroyRoom,

        // Db
        Persistence,
    }
}