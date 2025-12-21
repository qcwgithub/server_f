namespace Data
{
    public class ServiceConnection : DirectConnection
    {
        public readonly ServiceType serviceType;
        public readonly int serviceId;
        public bool remoteWillShutdown;

        public ServiceConnection(ServiceType serviceType, int serviceId, ProtocolClientData socket, bool isConnector) : base(socket, isConnector)
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
    }
}