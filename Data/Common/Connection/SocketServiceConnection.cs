using System.Net.Sockets;

namespace Data
{
    public class SocketServiceConnection : SocketConnection, ServiceConnection
    {
        ServiceType? _serviceType;
        int? _serviceId;
        public bool remoteWillShutdown { get; set; }

        // Connector
        public SocketServiceConnection(IConnectionCallbackProvider callbackProvider, string ip, int port, ServiceType serviceType, int serviceId) : base(callbackProvider, ip, port)
        {
            _serviceType = serviceType;
            _serviceId = serviceId;
        }

        // Acceptor
        public SocketServiceConnection(IConnectionCallbackProvider callbackProvider, Socket socket, bool forClient) : base(callbackProvider, socket, forClient, startRecv: false)
        {
            _serviceType = null;
            _serviceId = null;

            // 手动 StartRecv，否则会在基类构造函数里直接就收到消息
            this.StartRecv();
        }

        public bool knownWho
        {
            get
            {
                return _serviceType != null;
            }
        }

        public ServiceType serviceType
        {
            get
            {
                if (_serviceType == null)
                {
                    throw new InvalidOperationException("Service type is not set.");
                }
                return _serviceType.Value;
            }
            set
            {
                _serviceType = value;
            }
        }

        public int serviceId
        {
            get
            {
                if (_serviceId == null)
                {
                    throw new InvalidOperationException("Service ID is not set.");
                }
                return _serviceId.Value;
            }
            set
            {
                _serviceId = value;
            }
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