using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Data
{
    public class TcpListenerData
    {
        public bool isForClient;
        public ITcpListenerCallbackProvider callbackProvider;
        public ITcpListenerCallback callback => this.callbackProvider.GetTcpListenerCallback();
        public Socket socket;
        public bool closed;
        public SocketAsyncEventArgs listenSocketArg;
        public bool accepting = false;

        // public TcpClientData connectorData;
        // public TcpClientData acceptorData;

        // 此函数是多线程，因此必须放在 Data
        public void _onComplete(object sender, SocketAsyncEventArgs e)
        {
            ET.ThreadSynchronizationContext.Instance.Post(OnComplete, e);
        }
        public void Close()
        {
            if (this.closed)
            {
                // this.logError(this, $"call close on socketId({this.socketId}) with reason({reason}), but this.closed is true!");
                return;
            }
            // this.logInfo(this, $"call close on socketId({this.socketId}) with reason({reason})");
            this.closed = true;

            // doesn't need to call Shutdown on a listener socket. according to:
            // https://stackoverflow.com/questions/23963359/socket-shutdown-throws-socketexception
            if (this.socket.Connected)
            {
                this.socket.Shutdown(SocketShutdown.Both);
            }
            this.socket.Close();
            this.socket = null;

            this.listenSocketArg.Completed -= this._onComplete;
            this.listenSocketArg.Dispose();
            this.listenSocketArg = null;
        }

        public void Listen(int port)
        {
            if (port <= 0)
            {
                throw new Exception("TcpListenerData.Listen(): port <= 0");
            }

            var address = IPAddress.IPv6Any;
            this.socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.listenSocketArg = new SocketAsyncEventArgs();
            this.listenSocketArg.Completed += this._onComplete;

            // this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            // this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            // object ipv6Only = this.socket.GetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only);
            this.socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);

            try
            {
                this.socket.Bind(new IPEndPoint(address, port));
            }
            catch (Exception ex)
            {
                callback.LogError("TcpListenerData.Listen Bind Exception, port: " + port);
                throw ex;
            }
            this.socket.Listen(1000);
        }

        public void OnComplete(object _e)
        {
            var e = (SocketAsyncEventArgs)_e;
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    this.OnAcceptComplete();
                    break;
                default:
                    throw new Exception($"socket accept error: {e.LastOperation}");
            }
        }

        void OnAcceptComplete()
        {
            this.accepting = false;

            if (this.closed)
            {
                return;
            }

            ITcpListenerCallback callback = this.callbackProvider.GetTcpListenerCallback();

            try
            {
                callback.OnAcceptComplete(this, this.listenSocketArg);
            }
            catch (Exception ex)
            {
                callback.LogError("callback.OnAcceptComplete exception: " + ex);
            }
            finally
            {
                if (!this.closed)
                {
                    // continue accept
                    this.Accept();
                }
            }
        }

        bool acceptStarted = false;
        public void StartAccept()
        {
            if (this.acceptStarted)
            {
                return;
            }
            this.acceptStarted = true;
            this.Accept();
        }

        void Accept()
        {
            this.accepting = true;
            this.listenSocketArg.AcceptSocket = null;
            bool completed = !this.socket.AcceptAsync(this.listenSocketArg);
            if (completed)
            {
                this.OnAcceptComplete();
            }
        }
    }
}