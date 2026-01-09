using System.Net.Sockets;

namespace Data
{
    public class SocketConnection : IConnection, IProtocolClientCallback
    {
        public readonly ServiceData serviceData;
        public ProtocolClientData socket;
        public readonly bool isConnector;

        // Connector
        public SocketConnection(ServiceData serviceData, string ip, int port)
        {
            this.serviceData = serviceData;

            this.socket = new TcpClientData(this, this.serviceData.socketId++, ip, port);
            this.socket.customData = this;

            this.isConnector = true;
        }

        // Acceptor
        public SocketConnection(ServiceData serviceData, Socket socket, bool forClient)
        {
            this.serviceData = serviceData;

            // !
            socket.NoDelay = true;

            this.socket = new TcpClientData(this, this.serviceData.socketId++, forClient, socket);
            this.socket.customData = this;

            this.isConnector = false;
        }

        //////// IProtocolClientCallback ////////
        IMessagePacker IProtocolClientCallback.GetMessagePacker()
        {
            return this.serviceData.serverData.messagePacker;
        }
        void IProtocolClientCallback.LogError(ProtocolClientData data, string str)
        {
            this.serviceData.logger.Error(str);
        }
        void IProtocolClientCallback.LogError(ProtocolClientData data, string str, Exception ex)
        {
            this.serviceData.logger.Error(str, ex);
        }
        void IProtocolClientCallback.LogInfo(ProtocolClientData data, string str)
        {
            this.serviceData.logger.Info(str);
        }
        void IProtocolClientCallback.OnConnectComplete(ProtocolClientData data, bool success)
        {
            if (!success)
            {
                data.Close(ProtocolClientData.CloseReason.OnConnectComplete_false);
                return;
            }

            if (data.customData == null)
            {
                this.service.logger.Error("OnConnectComplete data.customData == null");
                return;
            }
        }
        void IProtocolClientCallback.OnCloseComplete(ProtocolClientData data)
        {

        }
        void IProtocolClientCallback.ReceiveFromNetwork(ProtocolClientData data, int seq, MsgType msgType, ArraySegment<byte> msg, ReplyCallback cb)
        {

        }
        //////// IProtocolClientCallback ////////

        public bool isAcceptor
        {
            get
            {
                return !this.isConnector;
            }
        }

        public void Connect()
        {
            this.socket.Connect();
        }

        public bool IsConnecting()
        {
            return this.socket.IsConnecting();
        }

        public bool IsConnected()
        {
            return this.socket.IsConnected();
        }

        public int GetConnectionId()
        {
            return this.socket.GetSocketId();
        }

        public bool IsClosed()
        {
            return this.socket.IsClosed();
        }

        public void Close(string reason)
        {
            this.socket.Close(reason);
        }

        public string? closeReason
        {
            get
            {
                return this.socket.closeReason;
            }
        }

        public void Send(MsgType msgType, object msg, ReplyCallback? cb, int? pTimeoutS)
        {
            if (!this.IsConnected())
            {
                if (cb != null)
                {
                    cb(ECode.NotConnected, default);
                }
                return;
            }

            var seq = this.serviceData.msgSeq++;
            if (seq <= 0)
            {
                seq = 1;
            }

            ArraySegment<byte> msgBytes = MessageTypeConfigData.SerializeMsg(msgType, msg);
            this.socket.SendBytes(msgType, msgBytes, seq, cb, pTimeoutS);
        }
    }
}