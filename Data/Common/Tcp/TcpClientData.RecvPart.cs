using System.Net.Sockets;

namespace Data
{
    public partial class TcpClientData
    {
        public class RecvPart
        {
            TcpClientData parent;
            SocketAsyncEventArgs _innArgs;
            byte[] recvBuffer;
            int recvOffset;
            bool isAcceptor;
            int identityVerified;

            public RecvPart(TcpClientData parent, bool isAcceptor)
            {
                this.parent = parent;
                this._innArgs = new SocketAsyncEventArgs();
                this._innArgs.Completed += this.OnSomethingComplete;
                this.recvBuffer = new byte[8192];
                this.recvOffset = 0;
                this.isAcceptor = isAcceptor;
            }

            public void Cleanup()
            {
                this._innArgs.Completed -= this.OnSomethingComplete;
                this._innArgs.Dispose();
                this._innArgs = null;

                this.recvBuffer = null;
            }

            // 只调用一次
            public void StartRecv()
            {
                this.PerformRecv();
            }

            // 自循环调用，不会外部调用
            void PerformRecv()
            {
                this.RecvAsync(this.recvBuffer, this.recvOffset, this.recvBuffer.Length - this.recvOffset);
            }

            void RecvAsync(byte[] buffer, int offset, int count)
            {
                try
                {
                    if (!this.parent.TryIncreaseIORef())
                    {
                        return;
                    }

                    this._innArgs.SetBuffer(buffer, offset, count);
                    bool completed = !this.parent.socket.ReceiveAsync(this._innArgs);
                    if (completed)
                    {
                        this.OnRecvComplete(this._innArgs);
                    }
                }
                catch (Exception e)
                {
                    this.parent.Close("recvAsync Exception " + e);
                }
            }

            public enum VerifyIdentityResult
            {
                HalfSuccess,
                Success,
                Failed,
            }

            VerifyIdentityResult VerifyIdentity(byte[] buffer, int offset, int count, out int identityLength)
            {
                identityLength = s_identity.Length;
                if (s_identity.Length == 0)
                {
                    return VerifyIdentityResult.Success;
                }

                var r = VerifyIdentityResult.Success;
                for (int i = 0; i < s_identity.Length; i++)
                {
                    if (count <= i)
                    {
                        r = VerifyIdentityResult.HalfSuccess;
                        break;
                    }

                    if (buffer[offset + i] != Convert.ToByte(s_identity[i]))
                    {
                        r = VerifyIdentityResult.Failed;
                        break;
                    }
                }

                if (r == VerifyIdentityResult.Failed)
                {
                    this.parent.callback.LogInfo($"{this.GetType().Name} verify identity failed, close socket!");
                }
                else if (r == VerifyIdentityResult.Success)
                {
                    Interlocked.Exchange(ref this.identityVerified, 1);
                }

                return r;
            }

            void OnRecvComplete(SocketAsyncEventArgs e)
            {
                try
                {
                    if (e.SocketError != SocketError.Success)
                    {
                        this.parent.Close("onRecvComplete SocketError." + e.SocketError);
                        return;
                    }

                    // https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.socketasynceventargs.bytestransferred?view=netcore-3.1
                    // If zero is returned from a read operation, the remote end has closed the connection.
                    if (e.BytesTransferred == 0)
                    {
                        this.parent.Close("onRecvComplete e.BytesTransferred == 0");
                        return;
                    }

                    if (this.parent.IsClosing())
                    {
                        return;
                    }

                    this.recvOffset += e.BytesTransferred;
                    int offset = 0;
                    int count = this.recvOffset;

                    if (this.isAcceptor && Volatile.Read(ref this.identityVerified) != 1)
                    {
                        var r = this.VerifyIdentity(this.recvBuffer, offset, count, out int identityLength);
                        if (r == VerifyIdentityResult.Success)
                        {
                            offset += identityLength;
                            count -= identityLength;
                        }
                        else if (r == VerifyIdentityResult.Failed)
                        {
                            this.parent.Close("VerifyIdentityFailed");
                        }
                    }

                    if (!this.isAcceptor || Volatile.Read(ref this.identityVerified) == 1)
                    {
                        int used = this.parent.callback.OnReceive(this.recvBuffer, offset, count);
                        offset += used;
                        count -= used;
                    }

                    if (!this.parent.IsClosing())
                    {
                        if (offset > 0)
                        {
                            Array.Copy(this.recvBuffer, offset, this.recvBuffer, 0, count);
                            this.recvOffset = count;
                        }

                        if (this.recvOffset >= this.recvBuffer.Length)
                        {
                            var newBuffer = new byte[this.recvBuffer.Length * 2];
                            Array.Copy(this.recvBuffer, newBuffer, this.recvOffset);
                            this.recvBuffer = newBuffer;
                        }

                        // continue recv
                        this.PerformRecv();
                    }
                }
                catch (Exception ex)
                {
                    this.parent.callback.LogError("OnRecvComplete " + ex);
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

            void OnSomethingComplete(object? sender, SocketAsyncEventArgs e)
            {
                try
                {
                    switch (e.LastOperation)
                    {
                        case SocketAsyncOperation.Receive:
                            {
                                this.OnRecvComplete(e);
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