using Data;

namespace Script
{
    public class GatewayProtocolClientScriptForC : ProtocolClientScript
    {
        public GatewayProtocolClientScriptForC(Server server, Service service) : base(server, service)
        {
        }

        public override async void DispatchNetwork(ProtocolClientData data, int seq, MsgType msgType, ArraySegment<byte> msgBytes, Action<ECode, byte[]>? reply)
        {
            var connection = (GatewayUserConnection)data.customData;

            (ECode e, byte[] resBytes) = await this.service.dispatcher.Dispatch(connection, msgType, msgBytes);
            if (reply != null)
            {
                reply(e, resBytes);
            }
        }

        public override void OnConnectComplete(ProtocolClientData data, bool success)
        {
            if (!success)
            {
                data.Close(ProtocolClientData.CloseReason.OnConnectComplete_false);
                return;
            }

            if (data.customData == null)
            {
                MyDebug.Assert(false, "data.customData == null");
                return;
            }

            var connection = (GatewayUserConnection)data.customData;

            var msg = new MsgOnConnectComplete();
            this.service.dispatcher.Dispatch<MsgOnConnectComplete, ResOnConnectComplete>(connection, MsgType._OnConnectComplete, msg).Forget();
        }

        public override void OnCloseComplete(ProtocolClientData data)
        {
            if (data.customData == null)
            {
                return;
            }

            var connection = (GatewayUserConnection)data.customData;

            var msg = new MsgConnectionClose();
            this.service.dispatcher.Dispatch<MsgConnectionClose, ResConnectionClose>(connection, MsgType._OnConnectionClose, msg).Forget();
        }
    }
}