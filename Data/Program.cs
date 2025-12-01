using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using log4net;
using System.Runtime.CompilerServices;
using System.Net.NetworkInformation;
using System.Diagnostics;
using log4net.Core;

namespace Data
{
    // https://docs.microsoft.com/en-us/dotnet/standard/assembly/unloadability
    class MyAssemblyLoadContext : AssemblyLoadContext
    {
        public MyAssemblyLoadContext() : base(isCollectible: true)
        {
        }

        protected override Assembly Load(AssemblyName name)
        {
            // As you can see, the Load method returns null.
            // That means that all the dependency assemblies are loaded into the default context,
            // and the new context contains only the assemblies explicitly loaded into it.
            return null;
        }
    }

    public class IServerAndContextWeakRef
    {
        public IServer iserver;
        public int seq;
        public Version version;
        public WeakReference contextWeakRef;
    }

    public class Program
    {
        public static Dictionary<string, string> s_args;

        public static string s_inIp;
        public static string s_inIpV6;

        public static void s_InitInIp()
        {
            s_inIp = null;
            s_inIpV6 = null;

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus != OperationalStatus.Up)
                {
                    continue;
                }

                foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                {
                    if (IPAddress.IsLoopback(ip.Address))
                    {
                        continue;
                    }

                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        s_inIp = ip.Address.ToString();
                    }
                    else if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        s_inIpV6 = ip.Address.ToString();
                    }
                }
            }

            if (string.IsNullOrEmpty(s_inIp) && string.IsNullOrEmpty(s_inIpV6))
            {
                LogStartError("string.IsNullOrEmpty(s_inIp) && string.IsNullOrEmpty(s_inIpV6)");
                return;
            }
        }

        public static int s_scriptDllSeq = 0;
        public static List<IServerAndContextWeakRef> s_assemblyLoadContextRefs;
        public static IServer LastIServer()
        {
            if (s_assemblyLoadContextRefs == null || s_assemblyLoadContextRefs.Count == 0)
            {
                return null;
            }

            var last = s_assemblyLoadContextRefs[s_assemblyLoadContextRefs.Count - 1];
            return last.iserver;
        }
        public static string s_scriptDllPath;
        public static string s_scriptPdbPath;
        public static ServerData s_serverData;
        public static DateTime s_checkTime;
        static List<Exception> s_ioThreadExceptions = new List<Exception>();
        static int s_ioThreadExceptionsCounter = 0;

        static string GetScriptDllPath()
        {
            string scriptDllPath;

            OperatingSystem os = Environment.OSVersion;
            if (os.Platform == PlatformID.Win32NT)
            {
                string workingDir = Directory.GetCurrentDirectory().Replace('\\', '/');
                string dataDllPath = Assembly.GetExecutingAssembly().Location.Replace('\\', '/');

                if (!dataDllPath.StartsWith(workingDir))
                {
                    LogStartError($"!dataDllPath({dataDllPath}).StartsWith(workingDir({workingDir}))");
                    return null;
                }

                string postfix = dataDllPath.Substring(workingDir.Length)
                    .Replace("/Data/", "/Script/")
                    .Replace("/Data.dll", "/Script.dll");

                scriptDllPath = workingDir + postfix;
            }
            else
            {
                scriptDllPath = "./Script.dll";
            }

            if (!File.Exists(scriptDllPath))
            {
                LogStartError($"!File.Exists({scriptDllPath})");
                return null;
            }
            return scriptDllPath;
        }

        static void CheckMsgTypeDuplicate()
        {
            MsgType[] all = (MsgType[])Enum.GetValues(typeof(MsgType));
            for (int i = 0; i < all.Length; i++)
            {
                for (int j = i + 1; j < all.Length; j++)
                {
                    if (all[j] == all[i] && all[i] != MsgType.ClientStart)
                    {
                        string message = "MsgType has duplicate value: " + (int)all[j];
                        throw new Exception(message);
                    }
                }
            }
        }

        static Log4netCreation misc_log4netCreation;
        public static ILog misc_logger;
        public static void LogStartError(string s, bool throw_ = true)
        {
            LogError(s, null);
            if (throw_)
            {
                throw new Exception(s);
            }
        }

        public static bool IsWindows()
        {
            OperatingSystem os = Environment.OSVersion;
            return os.Platform == PlatformID.Win32NT;
        }

        public static void Main(string[] args)
        {
            Main2(args, null);
        }

        static long s_frame = 0;
        public static void Main2(string[] args, IServer iserver)
        {
            foreach (string arg in args)
            {
                if (arg.StartsWith("program="))
                {
                    string program = arg.Substring("program=".Length);
                    if (program == "linux")
                    {
                        LinuxProgram.s_argMap = ParseArguments(args);
                        new LinuxProgram().Ask();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Unknown program: " + program);
                    }
                }
            }

#if DEBUG
            CheckMsgTypeDuplicate();
            DirtyElementTypeExt.CheckValueNotChange();
#endif

            // 异步方法全部会回掉到主线程
            ET.ThreadSynchronizationContext.CreateInstance();
            SynchronizationContext.SetSynchronizationContext(ET.ThreadSynchronizationContext.Instance);

            // unhandled exception
            System.AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhandledException);

            Console.WriteLine();
            Console.WriteLine("**** Data.dll");
#if DEBUG
            Console.WriteLine("**** Configuration: Debug");
#else
            Console.WriteLine("**** Configuration: Release");
#endif

            Console.WriteLine("**** Working Directory: " + Environment.CurrentDirectory);
            for (int i = 0; i < args.Length; i++)
            {
                Console.WriteLine("**** Arguments[{0}] {1}", i, args[i]);
            }

            misc_log4netCreation = new Log4netCreation();
            misc_log4netCreation.Create("miscRepo", Log4netCreation.LOCAL_LOG_DIR, new List<string> { "Misc" }, new List<bool> { false }, new ConfigLoader().log4netConfigXml, false);
            misc_logger = misc_log4netCreation.GetLogger("Misc");

            s_args = ParseArguments(args);
#if DEBUG
            if (s_args.TryGetValue("ServerConfigDir", out string serverConfigDir))
            {
                ConfigLoader.ServerConfigDir = serverConfigDir;
            }
#endif

            s_InitInIp();
            Console.WriteLine("**** ip: " + s_inIp);
            Console.WriteLine("**** ipv6: " + s_inIpV6);

            // Windows 可在控制按 Ctrl+C 退出
            if (IsWindows())
            {
                CtrlCHandler.Register(s_OnCtrlC);
            }
            else
            {
                // Linux 响应 Docker SIGTERM
                AppDomain.CurrentDomain.ProcessExit += s_ProcessExit;
            }

            try
            {
                s_serverData = new ServerData(s_args);
            }
            catch (Exception ex)
            {
                LogError("new DataEntry exception", ex);
#if DEBUG
                Console.Write("Press any key to exit...");
                Console.ReadKey();
#endif
                Environment.Exit(1);
            }

            s_assemblyLoadContextRefs = new List<IServerAndContextWeakRef>();

            s_scriptDllPath = GetScriptDllPath();
            Console.WriteLine("**** scriptDll: " + s_scriptDllPath);
            s_scriptPdbPath = s_scriptDllPath.Substring(0, s_scriptDllPath.Length - 4) + ".pdb";
            LoadScriptDll(iserver);

            while (true)
            {
                try
                {
                    Thread.Sleep(1);
                    s_frame++;

                    IServer last1 = LastIServer();
                    if (last1 != null)
                    {
                        last1.FrameStart(s_frame);
                    }

                    ET.ThreadSynchronizationContext.Instance.Update();

                    var refs = s_assemblyLoadContextRefs;
                    if (refs.Count > 1)
                    {
                        DateTime now = DateTime.Now;
                        var sb = new StringBuilder();
                        if (now.Subtract(s_checkTime).TotalSeconds > 1)
                        {
                            s_checkTime = now;
                            for (int i = 0; i < refs.Count; i++)
                            {
                                if (!refs[i].contextWeakRef.IsAlive)
                                {
                                    refs.RemoveAt(i);
                                    i--;
                                }
                                else
                                {
                                    sb.AppendFormat("Seq {0} V{1} ", refs[i].seq, refs[i].version);
                                }
                            }
                            LogInfo("AssemblyLoadContextRefs.Count = " + (refs.Count) + ", " + sb.ToString());
                        }
                    }

                    s_ioThreadExceptionsCounter++;
                    if (s_ioThreadExceptionsCounter > 1000)
                    {
                        s_ioThreadExceptionsCounter = 0;

                        s_ioThreadExceptions.Clear();
                        s_serverData.ioThread.GetExceptions(s_ioThreadExceptions);
                        if (s_ioThreadExceptions.Count > 0)
                        {
                            foreach (var ex in s_ioThreadExceptions)
                            {
                                LogError("io thread exception", ex);
                            }
                            s_ioThreadExceptions.Clear();
                        }
                    }

                    if (last1 != null)
                    {
                        IServer last2 = LastIServer();
                        if (last2 == last1)
                        {
                            last2.FrameEnd(s_frame);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogError("main loop exception", ex);
                }
            }
        }

        public static bool WriteScriptDllAndPdbFile(byte[] dllBytes, byte[] pdbBytes)
        {
            try
            {
                using (var dllFs = File.Open(s_scriptDllPath, FileMode.Open, FileAccess.Write, FileShare.None))
                {
                    using (var pdbFs = File.Open(s_scriptPdbPath, FileMode.Open, FileAccess.Write, FileShare.None))
                    {
                        dllFs.SetLength(dllBytes.Length);
                        dllFs.Seek(0, SeekOrigin.Begin);
                        dllFs.Write(dllBytes, 0, dllBytes.Length);

                        pdbFs.SetLength(pdbBytes.Length);
                        pdbFs.Seek(0, SeekOrigin.Begin);
                        pdbFs.Write(pdbBytes, 0, pdbBytes.Length);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                LogError("write script.dll exception", ex);

                return false;
            }
            // File.WriteAllBytes(s_scriptDllPath, dllBytes);
            // File.WriteAllBytes(s_scriptPdbPath, pdbBytes);
        }

        public static bool LoadScriptDll(IServer iserver)
        {
            try
            {
                if (iserver != null)
                {
                    s_scriptDllSeq++;
                    iserver.Attach(s_args, s_serverData, s_scriptDllSeq);
                    return true;
                }

                var bytes = File.ReadAllBytes(s_scriptDllPath);
                var symbols = File.ReadAllBytes(s_scriptPdbPath);

                IServerAndContextWeakRef last = null;
                if (s_assemblyLoadContextRefs.Count > 0)
                {
                    last = s_assemblyLoadContextRefs[s_assemblyLoadContextRefs.Count - 1];
                }

                var context = new MyAssemblyLoadContext();
                var weakRef = new WeakReference(context);

                Assembly assembly;
                using (var stream = new MemoryStream(bytes))
                {
                    using (var symbolStream = new MemoryStream(symbols))
                    {
                        assembly = context.LoadFromStream(stream, symbolStream);
                    }
                }

                iserver = (IServer)assembly.CreateInstance("Script.Server");
                s_scriptDllSeq++;

                if (iserver == null)
                {
                    LogError("iserver == null", null);
                }

                else if (last != null && last.iserver != null)
                {
                    Version lastV = last.iserver.GetScriptDllVersion();
                    Version currV = iserver.GetScriptDllVersion();
                    if (lastV.Major != currV.Major || lastV.Minor != currV.Minor)
                    {
                        throw new Exception($"lastV ({lastV.Major}.{lastV.Minor}) != currV ({currV.Major}.{currV.Minor})");
                    }
                }

                iserver.Attach(s_args, s_serverData, s_scriptDllSeq);
                assembly = null;
                //Console.WriteLine(">>>>>>>>>>"+s_scriptDllVersion);
                // context.Unload();

                s_assemblyLoadContextRefs.Add(
                    new IServerAndContextWeakRef
                    {
                        iserver = iserver,
                        seq = iserver.GetScriptDllSeq(),
                        version = iserver.GetScriptDllVersion(),
                        contextWeakRef = weakRef,
                    });

                if (last != null)
                {
                    last.iserver.Detach();
                    last.iserver = null; // 最好是置为 null...

                    // 这个可能会挂，不知道原因，Debug 模式
                    // 不过在 Debug 下，不调用 Unload ，测试加载 9000 次也没涨内存
                    // if (last.Item2.IsAlive)
                    // {
                    //     ((MyAssemblyLoadContext)last.Item2.Target).Unload();
                    // }
                }

                s_checkTime = DateTime.Now.AddSeconds(-1);

                return true;
            }
            catch (Exception ex)
            {
                LogError("LoadScriptDll exception ", ex);
                return false;
            }
        }

        public static void LogInfo(string message)
        {
            if (misc_logger != null)
            {
                misc_logger.Info(message);
            }
            else
            {
                Console.WriteLine(message);
            }
        }

        public static void LogError(string message, Exception ex)
        {
            if (misc_logger != null)
            {
                misc_logger.Error(message, ex);
            }
            else
            {
                Console.WriteLine(message + ex);
            }
        }

        static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogError("unhandled exception ", (Exception)e.ExceptionObject);
        }


        static Dictionary<string, string> ParseArguments(string[] args)
        {
            var argMap = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                int index = arg.IndexOf('=');
                if (index > 0)
                {
                    string key = arg.Substring(0, index);
                    string value = arg.Substring(index + 1);
                    // Console.WriteLine(key + ": " + value);
                    argMap.Add(key, value);
                }
            }
            return argMap;
        }

        static bool s_sending = false;
        public static void FeiShuLogAppender_Send(log4net.Core.LoggingEvent e)
        {
            if (s_serverData == null || s_serverData.feiShuSendErrorMessage == null || s_serverData.feiShuSendFatalMessage == null)
            {
                return;
            }

            if (s_sending)
            {
                // 防止循环
                return;
            }

            s_sending = true;

            // 如果下面出现了异常，就让 s_sending 永远是 false

            bool shouldSend = false;
            foreach (var baseServerData in s_serverData.serverDatas)
            {
                foreach (var kv2 in baseServerData.serverArg.serviceTypeAndIds)
                {
                    if (kv2.serviceType.ShouldSendFeiShuWhenLogError())
                    {
                        shouldSend = true;
                        break;
                    }
                }
                if (shouldSend)
                {
                    break;
                }
            }

            if (shouldSend)
            {
                string title = $"[{s_serverData.commonServerConfig.purpose}]{e.LoggerName}";
                string content = e.RenderedMessage;
                if (e.ExceptionObject != null)
                {
                    content += "\n" + e.ExceptionObject.ToString();
                }

                if (e.Level >= Level.Fatal)
                {
                    s_serverData.feiShuSendFatalMessage(title, content);

                }
                else
                {
                    s_serverData.feiShuSendErrorMessage(title, content);
                }
            }

            s_sending = false;
        }

        static void GracefullyExit(object state)
        {
            LogInfo("GracefullyExit");
            if (s_assemblyLoadContextRefs.Count > 0)
            {
                IServerAndContextWeakRef last = null;
                last = s_assemblyLoadContextRefs[s_assemblyLoadContextRefs.Count - 1];
                if (last.iserver != null)
                {
                    last.iserver.HandleEvent("exit");
                }
            }
        }

        static void s_OnCtrlC()
        {
            ET.ThreadSynchronizationContext.Instance.Post(GracefullyExit, null);
        }

        static void s_ProcessExit(object sender, EventArgs e)
        {
            ET.ThreadSynchronizationContext.Instance.Post(GracefullyExit, null);
        }
    }
}