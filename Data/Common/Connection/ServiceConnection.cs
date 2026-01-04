namespace Data
{
    public abstract class ServiceConnection : IConnection
    {
        public readonly ServiceType serviceType;
        public readonly int serviceId;
        public bool remoteWillShutdown;

        public ServiceConnection(ServiceType serviceType, int serviceId)
        {
            this.serviceType = serviceType;
            this.serviceId = serviceId;
        }

        public ServiceTypeAndId tai
        {
            get
            {
                return ServiceTypeAndId.Create(this.serviceType, this.serviceId);
            }
        }

        public abstract int GetConnectionId();
        public abstract void Connect();
        public abstract bool IsConnected();
        public abstract bool IsConnecting();
        public abstract void Send(MsgType msgType, object msg, ReplyCallback? cb, int? pTimeoutS);
        public abstract void Close(string reason);
        public abstract bool IsClosed();
        public abstract string? closeReason { get; }
    }
}