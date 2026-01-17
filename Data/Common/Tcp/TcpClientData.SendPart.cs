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

            public void Cleanup()
            {
                this._outArgs.Completed -= this.OnSomethingComplete;
                this._outArgs.Dispose();
            }

            public void Connect()
            {
                try
                {
                    if (!this.parent.TryIncreaseIORef())
                    {
                        return;
                    }
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
                try
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
                    else if (!this.parent.IsClosing())
                    {
                        this.parent.StartRecv();

                        if (Interlocked.CompareExchange(ref this.sending, 1, 0) == 0)
                        {
                            this.PerformSend();
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.parent.callback.LogError("OnConnectComplete " + ex);
                }
                finally
                {
                    // 说明：Decrease 发生在 Increase 之后，即确保没有下一步了，才可能变为 0
                    if (this.parent.DecreaseIORef() == 0 && this.parent.IsClosing())
                    {
                        this.parent.Cleanup();
                    }
                }
            }

            public void Send(byte[] bytes)
            {
                this.sendQueue.Enqueue(bytes);

                if (this.parent.forClient)
                {
                    var s_config = this.parent.callback.socketSecurityConfig;

                    if (this.sendQueue.Count > s_config.maxSendQueueCount)
                    {
                        this.parent.Close("send queue overflow");
                        return;
                    }

                    long sum = 0;
                    foreach (var q in this.sendQueue)
                    {
                        sum += q.Length;
                    }
                    if (sum > s_config.maxSendQueueBytes)
                    {
                        this.parent.Close("send queue too big");
                        return;
                    }
                }

                if (Interlocked.CompareExchange(ref this.sending, 1, 0) == 0)
                {
                    this.PerformSend();
                }
            }

            // 调用之前需确保 !IsClosing() && sending == 0
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
                    if (!this.parent.TryIncreaseIORef())
                    {
                        return;
                    }
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
                try
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
                catch (Exception ex)
                {
                    this.parent.callback.LogError("OnSendComplete " + ex);
                }
                finally
                {
                    if (this.parent.DecreaseIORef() == 0 && this.parent.IsClosing())
                    {
                        this.parent.Cleanup();
                    }
                }
            }

            void OnSomethingComplete(object? sender, SocketAsyncEventArgs e)
            {
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