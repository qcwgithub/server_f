using Data;

namespace Tool
{
    public class ToolConnection : SocketConnection
    {
        public ToolConnection(IConnectionCallbackProvider callbackProvider, string ip, int port) : base(callbackProvider, ip, port)
        {

        }

        public async Task<MyResponse> Request(MsgType msgType, object msg)
        {
            var tcs = new TaskCompletionSource<MyResponse>();

            this.Send(msgType, msg, (e, segment) =>
            {
                object res = MessageTypeConfigData.DeserializeRes(msgType, segment);
                tcs.SetResult(new MyResponse(e, res));
            });

            return await tcs.Task;
        }
    }
}