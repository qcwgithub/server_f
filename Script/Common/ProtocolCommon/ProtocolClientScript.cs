using Data;

namespace Script
{
    public abstract class ProtocolClientScript : ServiceScript<Service>, IProtocolClientCallback
    {
        public ProtocolClientScript(Server server, Service service) : base(server, service)
        {
        }

        public IMessagePacker GetMessagePacker()
        {
            return this.server.messagePacker;
        }

        public void LogError(ProtocolClientData data, string str)
        {
            this.service.logger.Error(str);
        }

        public void LogError(ProtocolClientData data, string str, Exception ex)
        {
            this.service.logger.Error(str, ex);
        }

        public void LogInfo(ProtocolClientData data, string str)
        {
            this.service.logger.Info(str);
        }

        public int nextSocketId
        {
            get
            {
                return this.service.data.socketId++;
            }
        }

        public int nextMsgSeq
        {
            get
            {
                return this.service.data.msgSeq++;
            }
        }

        public abstract void DispatchNetwork(ProtocolClientData data, int seq, MsgType msgType, ArraySegment<byte> msgBytes, Action<ECode, byte[]>? reply);
        public abstract void OnConnectComplete(ProtocolClientData data, bool success);
        public abstract void OnCloseComplete(ProtocolClientData data);
    }
}