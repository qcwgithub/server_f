using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace Data
{
    public partial class TcpClientData
    {
        public class SendPart
        {
            // IProtocolClientCallback
            // 实现者必须保证每一个函数都是线程安全
            TcpClientData parent;
            IPEndPoint? endPoint;
            int sending;
            ConcurrentQueue<byte[]> sendQueue;
            SocketAsyncEventArgs _outArgs;

            public SendPart(TcpClientData parent, IPEndPoint? endPoint)
            {
                this.parent = parent;
                this.endPoint = endPoint;
                Interlocked.Exchange(ref this.sending, 0);
                this.sendQueue = new();
                this._outArgs = new SocketAsyncEventArgs();
                this._outArgs.Completed += this.OnSomethingComplete;
            }

            public void Destroy()
            {
                Interlocked.Exchange(ref this.sending, 0);

                this._outArgs.Completed -= this.OnSomethingComplete;
                this._outArgs.Dispose();
                this._outArgs = null;

                this.sendQueue = null;
            }

            public void Connect()
            {
                try
                {
                    this._outArgs.RemoteEndPoint = this.endPoint;
                    bool completed = !this.parent.socket.ConnectAsync(this._outArgs);
                    if (completed)
                    {
                        this.OnConnectComplete(this._outArgs);
                    }
                }
                catch (SocketException ex)
                {
                    this.parent.callback.LogError("connect exception" + ex);
                }
            }

            protected byte[]? SendIdentity()
            {
                if (s_identity.Length == 0)
                {
                    return null;
                }
                var bytes = new byte[s_identity.Length];
                for (int i = 0; i < s_identity.Length; i++)
                {
                    bytes[i] = Convert.ToByte(s_identity[i]);
                }
                return bytes;
            }

            void OnConnectComplete(SocketAsyncEventArgs e)
            {
                e.RemoteEndPoint = null;

                bool success = e.SocketError == SocketError.Success;
                if (success)
                {
                    byte[]? bytes = this.SendIdentity();
                    if (bytes != null)
                    {
                        this.sendQueue.Enqueue(bytes);
                    }
                }

                this.parent.callback.OnConnect(success);
                if (!success)
                {
                    this.parent.Close(CloseReason.OnConnectComplete_false);
                }
                else
                {
                    this.parent.recvPart.StartRecv();

                    if (Interlocked.CompareExchange(ref this.sending, 1, 0) == 0)
                    {
                        this.PerformSend();
                    }
                }
            }

            public void Send(byte[] bytes)
            {
                this.sendQueue.Enqueue(bytes);

                if (Interlocked.CompareExchange(ref this.sending, 1, 0) == 0)
                {
                    this.PerformSend();
                }
            }

            void PerformSend()
            {
                if (!this.sendQueue.TryDequeue(out byte[]? bytes))
                {
                    Interlocked.Exchange(ref this.sending, 0);

                    // 关键：二次检查，防止 race
                    if (!sendQueue.IsEmpty &&
                        Interlocked.CompareExchange(ref this.sending, 1, 0) == 0)
                    {
                        PerformSend();
                    }
                    return;
                }

                try
                {
                    this._outArgs.SetBuffer(bytes, 0, bytes.Length);
                    bool completed = !this.parent.socket.SendAsync(this._outArgs);
                    if (completed)
                    {
                        this.OnSendComplete(this._outArgs);
                    }
                }
                catch (Exception e)
                {
                    this.parent.Close("sendAsync Exception " + e);
                }
            }

            void OnSendComplete(SocketAsyncEventArgs e)
            {
                if (e.SocketError != SocketError.Success)
                {
                    this.parent.Close("onTcpClientComplete_SocketAsyncOperation.Send_SocketError." + e.SocketError);
                    return;
                }

                if (e.BytesTransferred == 0)
                {
                    this.parent.Close("onTcpClientComplete_SocketAsyncOperation.Send_e.BytesTransferred == 0");
                    return;
                }

                // 不需要 Interlocked.Exchange(ref this.sending, 0);
                this.PerformSend();
            }

            void OnSomethingComplete(object? sender, SocketAsyncEventArgs e)
            {
                if (this.parent.IsClosed())
                {
                    return;
                }
                try
                {
                    switch (e.LastOperation)
                    {
                        case SocketAsyncOperation.Connect:
                            {
                                this.OnConnectComplete(e);
                            }
                            break;
                        case SocketAsyncOperation.Send:
                            {
                                this.OnSendComplete(e);
                            }
                            break;
                        default:
                            this.parent.callback.LogError("TcpClientData.onSomethingComplete default: " + e.LastOperation);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    this.parent.callback.LogError("onSomethingComplete " + ex);
                }
            }
        }
    }
}