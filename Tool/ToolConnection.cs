using Data;

namespace Tool
{
    public class ToolConnection : IConnectionCallbackProvider, IConnectionCallback
    {
        SocketConnection socketConnection;
        public ToolConnection(string ip, int port)
        {
            this.socketConnection = new SocketConnection(this, ip, port);
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

        TaskCompletionSource<bool> tcsConnect;

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

        public async Task<bool> Connect()
        {
            tcsConnect = new TaskCompletionSource<bool>();
            this.socketConnection.Connect();
            return await this.tcsConnect.Task;
        }

        public void Close()
        {
            this.socketConnection.Close("doesn't matter");
        }

        void IConnectionCallback.OnMsg(IConnection _, int seq, MsgType msgType, byte[] msgBytes, ReplyCallback? reply)
        {
            // var msg = MessageTypeConfigData.DeserializeMsg(msgType, msgBytes);
        }

        void IConnectionCallback.OnConnect(IConnection _, bool success)
        {
            Console.WriteLine($"OnConnect success? {success}");

            this.tcsConnect.SetResult(success);
        }

        void IConnectionCallback.OnClose(IConnection _)
        {
        }

        public async Task<MyResponse> Request(MsgType msgType, object msg)
        {
            var tcs = new TaskCompletionSource<MyResponse>();

            this.socketConnection.Send(msgType, msg, (e, segment) =>
            {
                object res = MessageTypeConfigData.DeserializeRes(msgType, segment);
                tcs.SetResult(new MyResponse(e, res));
            },
            null);

            return await tcs.Task;
        }

        IConnectionCallback IConnectionCallbackProvider.GetConnectionCallback(bool forClient)
        {
            return this;
        }
    }
}