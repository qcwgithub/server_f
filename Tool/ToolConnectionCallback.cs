using Data;

namespace Tool
{
    public class ToolConnectionCallback : IConnectionCallbackProvider, IConnectionCallback
    {
        ServerConfig.SocketSecurityConfig? _socketSecurityConfig;
        public ServerConfig.SocketSecurityConfig socketSecurityConfig
        {
            get
            {
                if (_socketSecurityConfig == null)
                {
                    _socketSecurityConfig = new ServerConfig.SocketSecurityConfig();
                }
                return _socketSecurityConfig;
            }
        }

        public void LogError(string str)
        {
            ConsoleEx.WriteLine(ConsoleColor.Red, str);
        }

        public void LogError(string str, Exception ex)
        {
            ConsoleEx.WriteLine(ConsoleColor.Red, str);
        }

        public void LogInfo(string str)
        {
            Console.WriteLine(str);
        }

        static BinaryMessagePacker s_binaryMessagePacker = new();
        public IMessagePacker messagePacker
        {
            get
            {
                return s_binaryMessagePacker;
            }
        }

        static int s_msgSeq = 1;
        public int nextMsgSeq
        {
            get
            {
                int seq = s_msgSeq++;
                if (seq <= 0)
                {
                    seq = 1;
                }
                return seq;
            }
        }

        public Action<MsgType, object, ReplyCallback?>? onMsg;

        void IConnectionCallback.OnMsg(IConnection _, int seq, MsgType msgType, byte[] msgBytes, ReplyCallback? reply)
        {
            var msg = MessageTypeConfigData.DeserializeMsg(msgType, msgBytes);
            this.onMsg?.Invoke(msgType, msg, reply);
        }

        public Action<bool>? onConnect;
        void IConnectionCallback.OnConnect(IConnection _, bool success)
        {
            Console.WriteLine($"OnConnect success? {success}");

            this.onConnect?.Invoke(success);
        }

        public System.Action? onClose;
        void IConnectionCallback.OnClose(IConnection _)
        {
            Console.WriteLine($"OnClose");
            this.onClose?.Invoke();
        }

        IConnectionCallback IConnectionCallbackProvider.GetConnectionCallback()
        {
            return this;
        }
    }
}