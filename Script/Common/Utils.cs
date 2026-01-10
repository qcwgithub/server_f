using Data;

namespace Script
{
    public static class Utils
    {
        public static async Task<MyResponse> Request(this IConnection connection, MsgType msgType, object msg)
        {
            var cs = new TaskCompletionSource<(ECode, byte[])>();

            connection.Send(msgType, msg, (e, segment) =>
            {
                bool success = cs.TrySetResult((e, segment));
                if (!success)
                {
                    Console.WriteLine("!cs.TrySetResult " + msgType);
                }
            },
            pTimeoutS: null);
    
            (ECode e, byte[] resSegment) = await cs.Task;

            object res = MessageTypeConfigData.DeserializeRes(msgType, resSegment);
            return new MyResponse(e, res);
        }

        public static bool IsWindows()
        {
            OperatingSystem os = Environment.OSVersion;
            return os.Platform == PlatformID.Win32NT;
        }
    }
}