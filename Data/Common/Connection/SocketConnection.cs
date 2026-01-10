using System.Net.Sockets;

namespace Data
{
    public class SocketConnection : IConnection, IProtocolClientCallback
    {
        public readonly ServiceData serviceData;
        public ProtocolClientData socket;
        public readonly bool isConnector;
        public readonly bool forClient;

        // Connector
        public SocketConnection(ServiceData serviceData, string ip, int port)
        {
            this.serviceData = serviceData;

            this.socket = new TcpClientData(this, this.serviceData.socketId++, ip, port);
            this.socket.customData = this;

            this.isConnector = true;
            this.forClient = false;
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
            this.forClient = forClient;
        }

        public bool isAcceptor
        {
            get
            {
                return !this.isConnector;
            }
        }

        IConnectionCallback callback
        {
            get
            {
                if (this.isConnector)
                {
                    return this.serviceData.connectionCallbackForS;
                }
                return this.forClient ? this.serviceData.connectionCallbackForC : this.serviceData.connectionCallbackForS;
            }
        }

        #region IProtocolClientCallback

        IMessagePacker IProtocolClientCallback.GetMessagePacker()
        {
            return this.serviceData.serverData.messagePacker;
        }
        void IProtocolClientCallback.LogError(string str)
        {
            this.serviceData.logger.Error(str);
        }
        void IProtocolClientCallback.LogError(string str, Exception ex)
        {
            this.serviceData.logger.Error(str, ex);
        }
        void IProtocolClientCallback.LogInfo(string str)
        {
            this.serviceData.logger.Info(str);
        }
        void IProtocolClientCallback.OnConnectComplete(bool success)
        {
            if (!success)
            {
                this.socket.Close(ProtocolClientData.CloseReason.OnConnectComplete_false);
                return;
            }
            this.callback.OnConnectComplete(this);
        }
        void IProtocolClientCallback.OnCloseComplete(ProtocolClientData data)
        {
            this.callback.OnCloseComplete(this);
        }
        void IProtocolClientCallback.ReceiveFromNetwork(int seq, MsgType msgType, ArraySegment<byte> msg, ReplyCallback cb)
        {
            this.callback.OnMsg(this, seq, msgType, msg, cb);
        }

        #endregion IProtocolClientCallback

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