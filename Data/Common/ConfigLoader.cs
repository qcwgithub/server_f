using System;
using System.IO;
using System.Text;
using System.Xml;
using Data;
using System.Collections.Generic;
using System.Linq;

namespace Data
{
    public class ConfigLoader
    {
        public static string ServerConfigDir = "ServerConfig";

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

        public string LoadConfigText(string f)
        {
            return ReadAllText("./config/" + f);
        }

        public T LoadConfigText<T>(string f)
        {
            return JsonUtils.parse<T>(this.LoadConfigText(f));
        }

        public static byte[] ReadAllBytes(string file)
        {
            using (var fileStream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        XmlElement parseConfigXml(string text)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(text);
            return doc.DocumentElement;
        }

        public T LoadInServerConfigJson<T>(string f)
        {
            return JsonUtils.parse<T>(ReadAllText($"./{ServerConfigDir}/" + f));
        }

        public string LoadInServerConfigText(string f)
        {
            return ReadAllText($"./{ServerConfigDir}/" + f);
        }

        public string LoadSql(string f)
        {
            return ReadAllText("./sql/" + f);
        }

        public XmlElement log4netConfigXml
        {
            get
            {
                string xmlText = LoadConfigText("log4netConfig.xml");
                return parseConfigXml(xmlText);
            }
        }

        public ServerConfig LoadServerConfig()
        {
            ServerConfig serverConfig = this.LoadInServerConfigJson<ServerConfig>("ServerConfig.json");
            serverConfig.Init();
            return serverConfig;
        }

        public const string GMAccountConfigFile = "config/GMAccount.csv";
        public Dictionary<string, Dictionary<string, GMAccountConfig>> LoadGMAccountConfigs()
        {
            var gmAccountConfigs = new Dictionary<string, Dictionary<string, GMAccountConfig>>();
            if (!File.Exists(GMAccountConfigFile))
            {
                return gmAccountConfigs;
            }

            string text = ReadAllText(GMAccountConfigFile);
            var helper = CsvUtils.Parse(text);
            while (helper.ReadRow())
            {
                var config = new GMAccountConfig();
                config.channel = helper.ReadString("channel");
                config.channelUserId = helper.ReadString("channelUserId");
                config.who = helper.ReadString("who");

                if (!gmAccountConfigs.TryGetValue(config.channel, out Dictionary<string, GMAccountConfig> dict))
                {
                    dict = gmAccountConfigs[config.channel] = new Dictionary<string, GMAccountConfig>();
                }

                dict[config.channelUserId] = config;
            }

            return gmAccountConfigs;
        }

        bool CheckServiceConfig_inPort_outPort(Dictionary<string, HashSet<int>> inPorts, Dictionary<string, HashSet<int>> outPorts, ServiceConfig sc, out string message)
        {
            HashSet<int> set;

            if (!inPorts.TryGetValue(sc.inIp, out set))
            {
                set = inPorts[sc.inIp] = new HashSet<int>();
            }

            if (!set.Add(sc.inPort))
            {
                message = $"inIp '{sc.inIp}' duplicate inPort '{sc.inPort}'";
                return false;
            }

            if (sc.outPort > 0)
            {
                if (!outPorts.TryGetValue(sc.inIp, out set))
                {
                    set = outPorts[sc.inIp] = new HashSet<int>();
                }

                if (!set.Add(sc.outPort))
                {
                    message = $"inIp '{sc.inIp}' duplicate outPort '{sc.outPort}'";
                    return false;
                }
            }

            message = null;
            return true;
        }

        bool CheckServiceConfigs(Dictionary<string, HashSet<int>> inPorts, Dictionary<string, HashSet<int>> outPorts, List<ServiceConfig> scs, out string message)
        {
            for (int i = 0; i < scs.Count; i++)
            {
                ServiceConfig sc = scs[i];
                if (!CheckServiceConfig_inPort_outPort(inPorts, outPorts, sc, out message))
                {
                    return false;
                }

                for (int j = i + 1; j < scs.Count; j++)
                {
                    if (scs[j].serviceId == sc.serviceId)
                    {
                        message = $"duplicate serviceId '{sc.serviceId}'";
                        return false;
                    }
                }
            }

            message = null;
            return true;
        }

        bool CheckAllServiceConfigs(
            List<ServiceConfig> allNormalServiceConfigs,
            out string message)
        {
            // 检查 inPort serviceId 重复
            var inPorts = new Dictionary<string, HashSet<int>>();
            var outPorts = new Dictionary<string, HashSet<int>>();

            if (!CheckServiceConfigs(inPorts, outPorts, allNormalServiceConfigs, out message))
            {
                return false;
            }

            message = null;
            return true;
        }

        // PKCastlesTool 也写死了这个名字
        public static string AllServiceConfigsFile => $"{ServerConfigDir}/AllServiceConfigs.csv";
        public bool LoadAllServiceConfigs(
            out List<ServiceConfig> allServiceConfigs,
            out string message)
        {
            allServiceConfigs = new List<ServiceConfig>();

            string text = ReadAllText(AllServiceConfigsFile);

            var helper = CsvUtils.Parse(text);
            while (helper.ReadRow())
            {
                var tai = ServiceTypeAndId.FromString(helper.ReadString("serviceTypeAndId"));

                var config = new ServiceConfig();
                config.serviceType = tai.serviceType;
                config.serviceId = tai.serviceId;

                config.inIp = helper.ReadString(nameof(config.inIp));
                config.inPort = helper.ReadInt(nameof(config.inPort));

                config.outIp = helper.ReadString(nameof(config.outIp));
                config.outPort = helper.ReadInt(nameof(config.outPort));

                allServiceConfigs.Add(config);
            }

            if (!CheckAllServiceConfigs(allServiceConfigs, out message))
            {
                return false;
            }

            return true;
        }
    }
}