using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Data
{
    public class TcpListenerData
    {
        ITcpListenerCallbackProvider callbackProvider;
        ITcpListenerCallback callback => this.callbackProvider.GetTcpListenerCallback(this);
        bool forClient;
        int port;

        int closing;
        public bool IsClosing()
        {
            return Volatile.Read(ref this.closing) == 1;
        }
        int cleanuped;
        int ioRef;
        public bool TryIncreaseIORef()
        {
            if (this.IsClosing())
            {
                return false;
            }

            Interlocked.Increment(ref this.ioRef);

            if (this.IsClosing())
            {
                Interlocked.Decrement(ref this.ioRef);
                return false;
            }

            return true;
        }
        public int DecreaseIORef()
        {
            return Interlocked.Decrement(ref this.ioRef);
        }

        Socket socket;
        SocketAsyncEventArgs listenSocketArg;

        public TcpListenerData(ITcpListenerCallbackProvider callbackProvider, bool forClient, int port)
        {
            this.callbackProvider = callbackProvider;
            this.forClient = forClient;
            this.port = port;
            if (port <= 0)
            {
                throw new Exception("TcpListenerData.ctor(): port <= 0");
            }

            Interlocked.Exchange(ref this.closing, 0);
            Interlocked.Exchange(ref this.cleanuped, 0);
            Interlocked.Exchange(ref this.ioRef, 0);

            var address = IPAddress.IPv6Any;
            this.socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.listenSocketArg = new SocketAsyncEventArgs();
            this.listenSocketArg.Completed += this.OnComplete;

            // this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            // this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            // object ipv6Only = this.socket.GetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only);
            this.socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
        }

        public void Listen()
        {
            try
            {
                var address = IPAddress.IPv6Any;
                this.socket.Bind(new IPEndPoint(address, this.port));
                this.socket.Listen(1000);
                this.PerformAccept();
            }
            catch (Exception ex)
            {
                callback.LogError("TcpListenerData.Listen Exception, port: " + this.port, ex);
            }
        }

        // 自循环调用，不会外部调用
        void PerformAccept()
        {
            if (!this.TryIncreaseIORef())
            {
                return;
            }

            this.listenSocketArg.AcceptSocket = null;
            bool completed = !this.socket.AcceptAsync(this.listenSocketArg);
            if (completed)
            {
                this.OnAcceptComplete();
            }
        }

        void OnComplete(object? sender, SocketAsyncEventArgs e)
        {
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
            if (this.IsClosing())
            {
                return;
            }

            try
            {
                if (this.listenSocketArg.SocketError == SocketError.Success)
                {
                    Socket? socket = this.listenSocketArg.AcceptSocket;
                    if (socket != null)
                    {
                        ITcpListenerCallback callback = this.callbackProvider.GetTcpListenerCallback(this);
                        callback.OnAccept(new ITcpListenerCallback.OnAcceptArg
                        {
                            forClient = this.forClient,
                            socket = socket
                        });
                    }
                }

                if (!this.IsClosing())
                {
                    this.PerformAccept();
                }
            }
            catch (Exception ex)
            {
                callback.LogError("callback.OnAcceptComplete exception: " + ex);
            }
            finally
            {
                // 说明：Decrease 发生在 Increase 之后，即确保没有下一步了，才可能变为 0
                if (this.DecreaseIORef() == 0 && this.IsClosing())
                {
                    this.Cleanup();
                }
            }
        }

        public void Close()
        {
            if (Interlocked.Exchange(ref this.closing, 1) != 0)
            {
                return;
            }

            if (Volatile.Read(ref this.ioRef) == 0)
            {
                this.Cleanup();
            }
        }

        void Cleanup()
        {
            if (Interlocked.Exchange(ref this.cleanuped, 1) != 0)
            {
                return;
            }

            // doesn't need to call Shutdown on a listener socket. according to:
            // https://stackoverflow.com/questions/23963359/socket-shutdown-throws-socketexception
            if (this.socket.Connected)
            {
                this.socket.Shutdown(SocketShutdown.Both);
            }
            this.socket.Close();

            this.listenSocketArg.Completed -= this.OnComplete;
            this.listenSocketArg.Dispose();
        }
    }
}