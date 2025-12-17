namespace Data
{
    public class ServiceConnection : DirectConnection
    {
        public ServiceTypeAndId? serviceTypeAndId;
        public bool remoteWillShutdown;

        // connector
        public ServiceConnection(ServiceType serviceType, int serviceId, ProtocolClientData socket) : base(socket, true)
        {
            this.serviceTypeAndId = new ServiceTypeAndId { serviceType = serviceType, serviceId = serviceId };
        }

        // acceptor
        public ServiceConnection(ProtocolClientData socket) : base(socket, false)
        {
            this.serviceTypeAndId = null;
        }
    }
}