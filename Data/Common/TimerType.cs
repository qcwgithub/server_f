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

        // Gateway
        DestroyGatewayUser,

        // Room
        SaveRoom,
        DestroyRoom,

        // Db
        Persistence,
    }
}