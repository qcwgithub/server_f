using System;

#if UNITY_2017_1_OR_NEWER
using ECode = System.Int32;
using MsgType = System.Int32;
#endif

namespace Data
{
    public interface IProtocolClientCallback
    {
        IMessagePacker GetMessagePacker(bool isMessagePack);

        int nextSocketId { get; }
        int nextMsgSeq { get; }

        void LogError(ProtocolClientData data, string str);
        void LogError(ProtocolClientData data, string str, Exception ex);
        void LogInfo(ProtocolClientData data, string str);

        void OnConnectComplete(ProtocolClientData data, bool success);

        void OnCloseComplete(ProtocolClientData data);

#if UNITY_2017_1_OR_NEWER
        void Dispatch(ProtocolClientData data, MsgType msgType, object msg, Action<ECode, object> cb);
#else
        void Dispatch(ProtocolClientData data, int seq, MsgType msgType, object msg, Action<ECode, object> cb);
#endif
    }

    public interface IProtocolClientCallbackProvider
    {
        IProtocolClientCallback GetProtocolClientCallback();
    }
}