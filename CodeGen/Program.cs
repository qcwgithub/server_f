using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using System.Linq;

namespace CodeGen
{
    public class Program
    {
        public static string FirstCharacterToLowercase(string str)
        {
            return char.ToLower(str[0]) + str.Substring(1);
        }

        static Dictionary<string, string> ParseArguments(string[] args)
        {
            var dict = new Dictionary<string, string>();
            foreach (var arg in args)
            {
                int i = arg.IndexOf('=');
                dict[arg.Substring(0, i)] = arg.Substring(i + 1);
            }
            return dict;
        }

        static string GetArg(Dictionary<string, string> argMap, string key)
        {
            string value;
            if (!argMap.TryGetValue(key, out value))
            {
                throw new Exception("missing argument: " + key);
            }
            return value;
        }

        static List<string> MatchMpc(string file)
        {
            List<string> ret = new List<string>();
            foreach (string[] array in GenMessageCode.s_extras)
            {
                ret.Add(array[0]);
            }

            // var allDataTypes = typeof(MsgLoginAAA).Assembly.GetTypes();
            string clientMpcText = File.ReadAllText(file);
            //{ typeof(global::Data.BMBattleInfo), 4 },
            Regex reg = new Regex(@"{ typeof\(global\:\:Data\.(\w+)\)\, \d+ }\,");
            Match match = reg.Match(clientMpcText);
            while (match.Success)
            {
                ret.Add(match.Groups[1].ToString());
                match = match.NextMatch();
            }
            return ret;
        }

        public static string ReadAllText(string file)
        {
            using (var fileStream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        static void AssertSorted(List<MessageCodeObj> list)
        {
            for (int i = 1; i < list.Count; i++)
            {
                if (list[i].value <= list[i - 1].value)
                {
                    throw new Exception($"{list[i].value} <= {list[i - 1].value}");
                }
            }
        }

        static List<MessageCodeObj> HandleMessageCode(Dictionary<string, string> argMap, string what, Func<string, bool> include, int startValue, Action<List<MessageCodeObj>> adjustFinals)
        {
            // 旧的
            List<MessageCodeObj> finals = GenMessageCode.Read(GetArg(argMap, what + "MCPath"))
                .Where(obj => include(obj.name))
                .ToList();

            // 新的
            List<string> currents = MatchMpc(GetArg(argMap, what + "Mpc"))
                .Where(s => include(s))
                .ToList();

            // 确保当前排序是从小到大的

            int nextValue;
            if (finals.Count > 0)
            {
                if (finals[0].value != startValue)
                {
                    throw new Exception("finals[0].value != startValue");
                }

                AssertSorted(finals);
                nextValue = finals[finals.Count - 1].value + 1;
            }
            else
            {
                nextValue = startValue;
            }

            // 有 -> 没有，注释
            foreach (MessageCodeObj final in finals)
            {
                if (!final.commented && !currents.Contains(final.name))
                {
                    final.commented = true;
                    Console.WriteLine("[{0}] ---- {1} {2}", what, final.name, final.value);
                }
            }

            foreach (string current in currents)
            {
                MessageCodeObj c = finals.Find(x => x.name == current);
                if (c == null)
                {
                    // 没有 -> 有，加上
                    c = MessageCodeObj.Create(false, current, nextValue);
                    nextValue++;
                    finals.Add(c);
                    Console.WriteLine("[{0}] ++++ {1} {2}", what, c.name, c.value);
                }
                else
                {
                    if (c.commented)
                    {
                        // 有 -> 有，取消注释
                        c.commented = false;
                        Console.WriteLine("[{0}] ++++ {1} {2}", what, c.name, c.value);
                    }
                }
            }

            AssertSorted(finals);

            adjustFinals?.Invoke(finals);

            AssertSorted(finals);

            // Console.Write($">>>> {what} MessageCode.cs...");
            GenMessageCode.Gen(finals, GetArg(argMap, what + "MCPath"));
            // Console.WriteLine("Done");

            // Console.Write($">>>> {what} BinaryMessagePackerGen.cs...");
            CreateFile.GenBinaryMessagePackerGen(finals, GetArg(argMap, what + "BMPGPath"));
            // Console.WriteLine("Done");

            // Console.Write($">>>> {what} JsonMessagePackerGen.cs...");
            CreateFile.GenJsonMessagePackerGen(finals, GetArg(argMap, what + "JMPGPath"));
            // Console.WriteLine("Done");

            return finals;
        }

        public static void Main(string[] args)
        {
            var argMap = ParseArguments(args);
            var action = argMap["action"];

            if (action.Contains("ServerData"))
            {
                ServerDataProgram.Do();
            }

            if (action.Contains("xinfo"))
            {
                Console.Write(">>>> Gen XInfo...");
                XInfoProgram.Do();
                Console.WriteLine("Done");
            }

            if (action.Contains("messagePack"))
            {
                // GenMessageCode.SaveInt("Data/Common/Gen/MessageCode.cs");
                // return;

                // client
                List<MessageCodeObj> client_finals = HandleMessageCode(argMap, "client", s => true, 0, null);

                // server
                HandleMessageCode(argMap, "server",
                    s =>
                    {
                        return !client_finals.Exists(_ => _.name == s);
                    },
                    10000,
                    server_finals =>
                    {
                        var temp = new List<MessageCodeObj>();
                        temp.AddRange(server_finals);

                        server_finals.Clear();
                        server_finals.AddRange(client_finals);
                        server_finals.AddRange(temp);
                    });
            }
        }
    }
}