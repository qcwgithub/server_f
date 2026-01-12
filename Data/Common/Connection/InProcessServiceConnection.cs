namespace Data
{
    public class InProcessServiceConnection : ServiceConnection
    {
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

        public InProcessServiceConnection(ServiceType serviceType, int serviceId)
        {
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

        public void Send(MsgType msgType, object msg, ReplyCallback? cb, int? pTimeoutS)
        {

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