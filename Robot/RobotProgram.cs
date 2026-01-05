namespace Data
{
    public class RobotProgram : IProtocolClientCallbackProvider, IProtocolClientCallback
    {
        public static void Main(string[] args)
        {
            new RobotProgram();
        }

        TcpClientData socket;
        BinaryMessagePacker binaryMessagePacker = new();
        int socketId = 1;
        int msgSeq = 1;
        public RobotProgram()
        {
            ET.ThreadSynchronizationContext.CreateInstance();
            SynchronizationContext.SetSynchronizationContext(ET.ThreadSynchronizationContext.Instance);

            this.socket = new TcpClientData();

            string ip = "localhost";
            int port = 8020;
            Console.WriteLine($"Connect {ip}:{port}");
            this.socket.ConnectorInit(this, ip, port);
            this.socket.Connect();
            while (true)
            {
                Thread.Sleep(1);
                ET.ThreadSynchronizationContext.Instance.Update();
            }
        }

        public IProtocolClientCallback? GetProtocolClientCallback(ProtocolClientData protocolClientData)
        {
            return this;
        }

        public IMessagePacker GetMessagePacker()
        {
            return this.binaryMessagePacker;
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
                return this.socketId++;
            }
        }

        public int nextMsgSeq
        {
            get
            {
                return this.msgSeq++;
            }
        }

        public virtual async void ReceiveFromNetwork(ProtocolClientData socket, int seq, MsgType msgType, ArraySegment<byte> msgBytes, ReplyCallback? reply)
        {
            var msg = MessageTypeConfigData.DeserializeMsg(msgType, msgBytes);

        }

        public async void OnConnectComplete(ProtocolClientData socket, bool success)
        {
            Console.WriteLine($"OnConnectComplete success? {success}");
            if (!success)
            {
                socket.Close(ProtocolClientData.CloseReason.OnConnectComplete_false);
                return;
            }

            await Task.Delay(1000);

            var msg = new MsgLogin();
            msg.version = string.Empty;
            msg.platform = "Windows";
            msg.channel = MyChannels.uuid;
            msg.channelUserId = "1000";

            Console.WriteLine($"Login channelUserId {msg.channelUserId}");

            byte[] msgBytes = MessageTypeConfigData.SerializeMsg(MsgType.Login, msg);

            socket.SendBytes(MsgType.Login, msgBytes, (ECode e, ArraySegment<byte> segments) =>
            {
                Console.WriteLine($"Login result, e = {e}");

                if (e == ECode.Success)
                {
                    var res = (ResLogin)MessageTypeConfigData.DeserializeRes(MsgType.Login, segments);
                    //
                }
            },
            null);
        }

        public void OnCloseComplete(ProtocolClientData socket)
        {
            Console.WriteLine($"OnCloseComplete");

            if (socket.customData == null)
            {
                return;
            }
        }
    }
}