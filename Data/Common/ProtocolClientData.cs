namespace Data
{
    public abstract class ProtocolClientData : IProtocolClient
    {
        protected readonly IProtocolClientCallback callback;

        #region variables
        protected readonly bool isConnector;
        protected bool isAcceptor => !this.isConnector;
        protected bool identityVerified;
        public abstract bool IsClosed();

        public abstract System.Net.EndPoint RemoteEndPoint { get; }
        #endregion

        public ProtocolClientData(IProtocolClientCallback output, bool isConnector)
        {
            this.callback = output;
            this.isConnector = isConnector;
        }

        public static string s_identity = "SceneHub";
        protected byte[]? SendIdentity()
        {
            if (s_identity.Length == 0)
            {
                return null;
            }
            var bytes = new byte[s_identity.Length];
            for (int i = 0; i < s_identity.Length; i++)
            {
                bytes[i] = Convert.ToByte(s_identity[i]);
            }
            return bytes;
        }

        public enum VerifyIdentityResult
        {
            HalfSuccess,
            Success,
            Failed,
        }

        protected VerifyIdentityResult VerifyIdentity(byte[] buffer, int offset, int count, out int identityLength)
        {
            identityLength = s_identity.Length;
            if (s_identity.Length == 0)
            {
                return VerifyIdentityResult.Success;
            }

            var r = VerifyIdentityResult.Success;
            for (int i = 0; i < s_identity.Length; i++)
            {
                if (count <= i)
                {
                    r = VerifyIdentityResult.HalfSuccess;
                    break;
                }

                if (buffer[offset + i] != Convert.ToByte(s_identity[i]))
                {
                    r = VerifyIdentityResult.Failed;
                    break;
                }
            }

            if (r == VerifyIdentityResult.Failed)
            {
                this.callback.LogInfo($"{this.GetType().Name} verify identity failed, close socket!");
            }
            else if (r == VerifyIdentityResult.Success)
            {
                this.identityVerified = true;
            }

            return r;
        }

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