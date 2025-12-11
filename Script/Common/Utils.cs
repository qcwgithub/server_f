using Data;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Web;

namespace Script
{
    public static class Utils
    {
        public static byte[] Serialize<T>(T msg)
        {
            return MessagePackSerializer.Serialize<T>(msg);
        }

        public static T Deserialize<T>(ArraySegment<byte> msg)
        {
            return MessagePackSerializer.Deserialize<T>(msg);
        }

        public static async Task<MyResponse<Res>> Request<Msg, Res>(this IConnection socket, MsgType msgType, byte[] msgBytes) where Res : class
        {
            var cs = new TaskCompletionSource<(ECode, ArraySegment<byte>)>();

            socket.SendBytes(msgType, msgBytes, (e, segment) =>
            {
                bool success = cs.TrySetResult((e, segment));
                if (!success)
                {
                    Console.WriteLine("!cs.TrySetResult " + msgType);
                }
            },
            pTimeoutS: null);
    
            (ECode e, ArraySegment<byte> segment) = await cs.Task;

            Res res = Deserialize<Res>(segment);
            return new MyResponse<Res>(e, res);
        }

        public static void Send(this IConnection socket, MsgType msgType, byte[] msgBytes)
        {
            socket.SendBytes(msgType, msgBytes, null, pTimeoutS: null);
        }

        public static async Task<MyResponse<Res>> Request<Msg, Res>(this IConnection socket, MsgType msgType, Msg msg) where Res : class
        {
            byte[] msgBytes = Serialize<Msg>(msg);
            return await Request<Msg, Res>(socket, msgType, msgBytes);
        }

        public static void Send<Msg>(this IConnection socket, MsgType msgType, Msg msg)
        {
            byte[] msgBytes = Serialize<Msg>(msg);
            Send(socket, msgType, msgBytes);
        }

        public static T CastObject<T>(object msg)
        {
            if (msg == null || !(msg is T))
            {
                string message = string.Empty;
                if (msg == null)
                {
                    message = string.Format("Can not convert null to '{0}'", typeof(T).Name);
                }
                else
                {
                    message = string.Format("Can not convert '{0}' to '{1}'", msg.GetType().Name, typeof(T).Name);
                }

                throw new InvalidCastException(message);
            }
            return (T)msg;
        }

        public static string BuildUri(string baseUrl, Dictionary<string, string> dict)
        {
            var builder = new UriBuilder(baseUrl);
            builder.Port = -1;
            var query = HttpUtility.ParseQueryString(builder.Query);
            foreach (var kv in dict)
            {
                query[kv.Key] = kv.Value;
            }
            builder.Query = query.ToString();
            string url = builder.ToString();
            return url;
        }

        public static bool IsWindows()
        {
            OperatingSystem os = Environment.OSVersion;
            return os.Platform == PlatformID.Win32NT;
        }

        public static void RunWithCatchException(log4net.ILog logger, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                logger.Error("RunWithCatchException exception: ", ex);
            }
        }
    }
}