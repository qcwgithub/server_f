namespace Data
{
    public interface ServiceConnection : IConnection
    {
        bool knownWho { get; }
        ServiceType serviceType { get; }
        int serviceId { get; }
        bool remoteWillShutdown { get; set; }

        ServiceTypeAndId tai { get; }

        void Connect();
        bool IsConnecting();
        void Close(string reason);
        bool IsClosed();
        string? closeReason { get; }
    }
}