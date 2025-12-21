namespace Data
{
    public class ServiceConnection : DirectConnection
    {
        public bool taiInited { get; private set; }

        ServiceType _serviceType;
        public ServiceType serviceType
        {
            get
            {
                MyDebug.Assert(this.taiInited);
                return _serviceType;
            }
        }

        int _serviceId;
        public int serviceId
        {
            get
            {
                MyDebug.Assert(this.taiInited);
                return _serviceId;
            }
        }

        public bool remoteWillShutdown;

        // connector
        public ServiceConnection(ServiceType serviceType, int serviceId, ProtocolClientData socket) : base(socket, true)
        {
            this.InitTai(serviceType, serviceId);
        }

        // acceptor
        public ServiceConnection(ProtocolClientData socket) : base(socket, false)
        {

        }

        public void InitTai(ServiceType serviceType, int serviceId)
        {
            MyDebug.Assert(!this.taiInited);
            this.taiInited = true;
            _serviceType = serviceType;
            _serviceId = serviceId;
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