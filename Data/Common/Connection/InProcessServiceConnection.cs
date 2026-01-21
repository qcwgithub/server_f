namespace Data
{
    public class InProcessServiceConnection : IServiceConnection
    {
        public bool isCommand
        {
            get
            {
                return false;
            }
        }

        public bool knownWho
        {
            get
            {
                return true;
            }
        }
        public ServiceType serviceType { get; private set; }
        public int serviceId { get; private set; }
        public bool remoteWillShutdown { get; set; }

        public ServiceTypeAndId tai
        {
            get
            {
                return ServiceTypeAndId.Create(this.serviceType, this.serviceId);
            }
        }

        public string identifierString
        {
            get
            {
                return this.tai.ToString();
            }
        }

        readonly IConnectionCallbackProvider callbackProvider;
        public InProcessServiceConnection(IConnectionCallbackProvider callbackProvider, ServiceType serviceType, int serviceId)
        {
            this.callbackProvider = callbackProvider;
            this.serviceType = serviceType;
            this.serviceId = serviceId;
        }

        public void Connect()
        {

        }

        public bool IsConnected()
        {
            return true;
        }

        public bool IsConnecting()
        {
            return false;
        }

        public void Send(MsgType msgType, object msg, ReplyCallback? cb)
        {
            this.callbackProvider.GetConnectionCallback().OnMsg(this, 0, msgType, msg, cb);
        }

        public void Close(string reason)
        {

        }

        public bool IsClosed()
        {
            return false;
        }

        public string? closeReason
        {
            get
            {
                return string.Empty;
            }
        }
    }
}