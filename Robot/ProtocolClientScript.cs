using Data;

namespace Script
{
    public abstract class ProtocolClientScript : IProtocolClientCallback
    {
        readonly BinaryMessagePacker binaryMessagePacker = new();
        int socketId = 1;
        int msgSeq = 1;

        public IMessagePacker GetMessagePacker()
        {
            return this.messagePacker;
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

        public virtual async void ReceiveFromNetwork(ProtocolClientData data, int seq, MsgType msgType, ArraySegment<byte> msgBytes, ReplyCallback? reply)
        {
            var msg = MessageConfigData.DeserializeMsg(msgType, msgBytes);
            
        }

        public void OnConnectComplete(ProtocolClientData data, bool success)
        {
            if (!success)
            {
                data.Close(ProtocolClientData.CloseReason.OnConnectComplete_false);
                return;
            }

            if (data.customData == null)
            {
                MyDebug.Assert(false, "data.customData == null");
                return;
            }

            this.service.OnConnectComplete((IConnection)data.customData).Forget();
        }

        public void OnCloseComplete(ProtocolClientData data)
        {
            if (data.customData == null)
            {
                return;
            }

            this.service.OnConnectionClose((IConnection)data.customData).Forget();
        }
    }
}