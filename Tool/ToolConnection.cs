using Data;

namespace Tool
{
    public class ToolConnection : IProtocolClientCallbackProvider, IProtocolClientCallback
    {
        static BinaryMessagePacker binaryMessagePacker = new();
        static int socketId = 1;
        static int msgSeq = 1;

        public IProtocolClientCallback? GetProtocolClientCallback(ProtocolClientData protocolClientData)
        {
            return this;
        }

        public IMessagePacker GetMessagePacker()
        {
            return binaryMessagePacker;
        }

        public void LogError(ProtocolClientData data, string str)
        {
            Console.WriteLine(str);
        }

        public void LogError(ProtocolClientData data, string str, Exception ex)
        {
            Console.WriteLine(str, ex);
        }

        public void LogInfo(ProtocolClientData data, string str)
        {
            Console.WriteLine(str);
        }

        public int nextSocketId
        {
            get
            {
                return socketId++;
            }
        }

        TcpClientData? socket;
        TaskCompletionSource<bool> tcsConnectComplete;
        public async Task<bool> Connect(string ip, int port)
        {
            if (this.socket != null)
            {
                this.Close();
            }
            tcsConnectComplete = new TaskCompletionSource<bool>();

            Console.WriteLine($"Connect {ip}:{port}");

            this.socket = new TcpClientData();
            this.socket.ConnectorInit(this, ip, port);
            this.socket.Connect();

            return await this.tcsConnectComplete.Task;
        }

        public void Close()
        {
            if (this.socket != null)
            {
                this.socket.Close(string.Empty);
                this.socket = null;
            }
        }

        public void ReceiveFromNetwork(ProtocolClientData socket, int seq, MsgType msgType, ArraySegment<byte> msgBytes, ReplyCallback? reply)
        {
            // var msg = MessageTypeConfigData.DeserializeMsg(msgType, msgBytes);
        }

        public void OnConnectComplete(ProtocolClientData socket, bool success)
        {
            Console.WriteLine($"OnConnectComplete success? {success}");
            if (!success)
            {
                socket.Close(ProtocolClientData.CloseReason.OnConnectComplete_false);
                this.socket = null;
            }

            this.tcsConnectComplete.SetResult(success);
        }

        public void OnCloseComplete(ProtocolClientData socket)
        {
        }

        public async Task<MyResponse> Request(MsgType msgType, object msg)
        {
            var tcs = new TaskCompletionSource<MyResponse>();

            ArraySegment<byte> msgBytes = MessageTypeConfigData.SerializeMsg(msgType, msg);
            this.socket.SendBytes(msgType, msgBytes, msgSeq++, (e, segment) =>
            {
                object res = MessageTypeConfigData.DeserializeRes(msgType, segment);
                tcs.SetResult(new MyResponse(e, res));
            },
            null);

            return await tcs.Task;
        }
    }
}