using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Data
{
    public struct stWaitingResponse
    {
        public Action<ECode, ArraySegment<byte>> callback;
        // public CancellationTokenSource source;
    }

    public abstract class ProtocolClientData : IConnection
    {
        public static MsgType MsgType_ClientStart
        {
            get
            {
                return MsgType.ClientStart;
            }
        }
        public static ECode ECode_NotConnected
        {
            get
            {
                return ECode.Server_NotConnected;
            }
        }
        public static ECode ECode_Timeout
        {
            get
            {
                return ECode.Server_Timeout;
            }
        }
        public static ECode ECode_Exception
        {
            get
            {
                return ECode.Exception;
            }
        }

        public IProtocolClientCallbackProvider? callbackProvider;
        public IProtocolClientCallback? callback => this.callbackProvider?.GetProtocolClientCallback();

        #region variables
        public bool isConnector;
        public bool isAcceptor => !this.isConnector;
        protected bool identityVerified;

        // 自定义的 id
        public int socketId;
        public int GetSocketId() => this.socketId;

        // when isAcceptor == true
        public bool oppositeIsClient;

        // when oppositeIsClient = false
        public bool oppositeIsService => !this.oppositeIsClient;

        public abstract bool IsConnecting();
        public abstract bool IsConnected();
        public abstract bool IsClosed();

        public bool sending;


        // when isConnectedFromClient == true
        public object? user;
        public long userId;
        public string? user_version;
        public long lastUserId;

        public int msgProcessing;
        public Dictionary<int, stWaitingResponse> waitingResponseDict = new Dictionary<int, stWaitingResponse>();

        public abstract System.Net.EndPoint RemoteEndPoint { get; }
        #endregion

        public static string s_identity = "pkcastles";
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

        public enum VerifyIdentityResult
        {
            HalfSuccess,
            Success,
            Failed,
        }
        protected VerifyIdentityResult VerifyIdentity(byte[] buffer, int offset, int count, out int identityLength)
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
                this.callback!.LogInfo(this, $"{this.GetType().Name} verify identity failed, close socket!");
            }
            else if (r == VerifyIdentityResult.Success)
            {
                this.identityVerified = true;
            }

            return r;
        }

        #region connect
        public abstract void Connect();
        #endregion

        #region send
        public abstract void SendBytes(MsgType msgType, byte[] msg, Action<ECode, ArraySegment<byte>>? cb, int? pTimeoutS);
        protected abstract void SendPacketIgnoreResult(int msgTypeOrECode, byte[] msg, int seq, bool requireResponse);
        protected abstract void SendRaw(byte[] buffer);

        #endregion

        #region recv

        protected void OnMsg(int seq, int code, ArraySegment<byte> msg, bool requireResponse)
        {
            try
            {
                /*
                if (msg != null && msg is string && (string)msg == "ping")
                {
                    // this.server.logger.info("receive ping, send pong");
                    this.SendOnePacket(0, "pong", 0, false);
                    return;
                }
                */

                //// 3 receive message
                if (seq > 0)
                {
                    MsgType msgType = (MsgType)code;
                    if (this.oppositeIsClient && msgType < MsgType_ClientStart)
                    {
                        this.callback!.LogError(this, "receive invalid message from client! " + msgType.ToString());
                        if (requireResponse)
                        {
                            this.SendPacketIgnoreResult((int)ECode_Exception, null, -seq, false);
                        }
                        return;
                    }

                    if (!requireResponse)
                    {
                        this.callback!.DispatchNetwork(this, seq, msgType, msg, null);
                    }
                    else
                    {
                        this.callback!.DispatchNetwork(this, seq, msgType, msg,
                            (ECode e2, byte[] msg2) =>
                            {
                                // 消息处理是异步的，在回复的时候，有可能已经断开了。因此这里要加个判断
                                if (!this.IsClosed())
                                {
                                    // Console.WriteLine("reply -seq = {0}, msgType = {1}", -seq, msgType);
                                    this.SendPacketIgnoreResult((int)e2, msg2, -seq, false);
                                }
                            });
                    }
                }
                //// 2 response message
                else if (seq < 0)
                {
                    // this.server.logger.Info("recv response " + eCode + ", " + msg);

                    ECode eCode = (ECode)code;

                    stWaitingResponse st;
                    if (this.waitingResponseDict.TryGetValue(-seq, out st))
                    {
                        this.waitingResponseDict.Remove(-seq);

                        // st.source.Cancel();
                        // st.source.Dispose();

                        // Console.WriteLine("--waiting {0}, -seq = {1}", this.waitingResponses.Count, seq);
                        st.callback(eCode, msg);
                    }
                    else
                    {
                        this.callback!.LogError(this, "No response fun for " + (-seq));
                    }
                }
                else
                {
                    this.callback!.LogError(this, "onMsg wrong seq: " + seq);
                }
            }
            catch (Exception ex)
            {
                this.callback!.LogError(this, "ProtocolClientData.OnMsg " + ex);
            }
        }

        #endregion

        #region close

        public static class CloseReason
        {
            public static readonly string OnConnectComplete_false = "OnConnectComplete_false";
        }
        public string? closeReason { get; protected set; }
        public abstract void Close(string reason);

        protected void TimeoutAllWaitings()
        {
            // timeout all waiting responses
            if (this.waitingResponseDict.Count > 0)
            {
                var list = new List<Action<ECode, ArraySegment<byte>>>();
                foreach (var kv in this.waitingResponseDict)
                {
                    // kv.Value.source.Cancel();
                    // kv.Value.source.Dispose();
                    list.Add(kv.Value.callback);
                }
                this.waitingResponseDict.Clear();
                foreach (var reply in list)
                {
                    reply(ECode_Timeout, null);
                }
            }
        }
        #endregion
    }
}