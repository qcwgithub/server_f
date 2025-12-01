using Data;
using System;
using System.Collections.Generic;
using System.Web;

namespace Script
{
    public class Utils
    {
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