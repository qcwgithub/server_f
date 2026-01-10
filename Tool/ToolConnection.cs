using Data;

namespace Tool
{
    public class ToolConnection : IProtocolClientCallback
    {
        static BinaryMessagePacker binaryMessagePacker = new();
        static int socketId = 1;
        static int msgSeq = 1;

        public void LogError(string str)
        {
            Console.WriteLine(str);
        }

        public void LogError(string str, Exception ex)
        {
            Console.WriteLine(str, ex);
        }

        public void LogInfo(string str)
        {
            Console.WriteLine(str);
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

            this.socket = new TcpClientData(this, socketId++, ip, port);
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

        public void OnReceive(int seq, MsgType msgType, ArraySegment<byte> msgBytes, ReplyCallback? reply)
        {
            // var msg = MessageTypeConfigData.DeserializeMsg(msgType, msgBytes);
        }

        public void OnConnect(bool success)
        {
            Console.WriteLine($"OnConnectComplete success? {success}");
            if (!success)
            {
                socket.Close(ProtocolClientData.CloseReason.OnConnectComplete_false);
                this.socket = null;
            }

            this.tcsConnectComplete.SetResult(success);
        }

        public void OnClose()
        {
        }

        public async Task<MyResponse> Request(MsgType msgType, object msg)
        {
            var tcs = new TaskCompletionSource<MyResponse>();

            byte[] msgBytes = MessageTypeConfigData.SerializeMsg(msgType, msg);
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