namespace Data
{
    public class InProcessServiceConnection : ServiceConnection
    {
        public InProcessServiceConnection(ServiceType serviceType, int serviceId) : base(serviceType, serviceId)
        {
        }

        public override int GetConnectionId()
        {
            return 0;
        }

        public override void Connect()
        {

        }

        public override bool IsConnected()
        {
            return true;
        }

        public override bool IsConnecting()
        {
            return false;
        }

        public override void Send(MsgType msgType, object msg, ReplyCallback? cb, int? pTimeoutS)
        {

        }

        public override void Close(string reason)
        {

        }

        public override bool IsClosed()
        {
            return false;
        }

        public override string? closeReason
        {
            get
            {
                return string.Empty;
            }
        }
    }
}