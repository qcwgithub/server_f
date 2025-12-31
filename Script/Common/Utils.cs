using Data;

namespace Script
{
    public static class Utils
    {
        public static async Task<MyResponse> Request(this IConnection connection, MsgType msgType, byte[] msgBytes)
        {
            var cs = new TaskCompletionSource<(ECode, ArraySegment<byte>)>();

            connection.SendBytes(msgType, msgBytes, (e, segment) =>
            {
                bool success = cs.TrySetResult((e, segment));
                if (!success)
                {
                    Console.WriteLine("!cs.TrySetResult " + msgType);
                }
            },
            pTimeoutS: null);
    
            (ECode e, ArraySegment<byte> resSegment) = await cs.Task;

            object res = MessageConfigData.DeserializeRes(msgType, resSegment);
            return new MyResponse(e, res);
        }

        public static void SendBytes(this IConnection connection, MsgType msgType, byte[] msgBytes, ReplyCallback? cb, int? pTimeoutS)
        {
            connection.SendBytes(msgType, msgBytes, cb, pTimeoutS);
        }

        public static async Task<MyResponse> Request(this IConnection connection, MsgType msgType, object msg)
        {
            byte[] msgBytes = MessageConfigData.SerializeMsg(msgType, msg);
            return await Request(connection, msgType, msgBytes);
        }

        public static void Send(this IConnection connection, MsgType msgType, object msg, ReplyCallback? cb, int? pTimeoutS)
        {
            byte[] msgBytes = MessageConfigData.SerializeMsg(msgType, msg);
            connection.SendBytes(msgType, msgBytes, cb, pTimeoutS);
        }

        public static bool IsWindows()
        {
            OperatingSystem os = Environment.OSVersion;
            return os.Platform == PlatformID.Win32NT;
        }
    }
}