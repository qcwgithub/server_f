namespace Data
{
    public abstract class ProtocolClientData : IProtocolClient
    {
        // IProtocolClientCallback
        // 实现者必须保证每一个函数都是线程安全
        protected readonly IProtocolClientCallback callback;

        #region variables
        protected readonly bool isConnector;
        protected bool isAcceptor => !this.isConnector;
        public abstract bool IsClosed();

        public abstract System.Net.EndPoint RemoteEndPoint { get; }
        #endregion

        public ProtocolClientData(IProtocolClientCallback output, bool isConnector)
        {
            this.callback = output;
            this.isConnector = isConnector;
        }

        public static string s_identity = "SceneHub";

        public abstract void Connect();
        public abstract void Send(byte[] bytes);

        #region close

        public static class CloseReason
        {
            public static readonly string OnConnectComplete_false = "OnConnectComplete_false";
        }
        public string? closeReason { get; protected set; }
        public abstract void Close(string reason);
        #endregion
    }
}