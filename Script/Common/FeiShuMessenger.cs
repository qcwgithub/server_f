using System.Threading.Tasks;
using System.Collections.Generic;
using Data;
using System.Net.Http;
using System.Text;
using System;

namespace Script
{
    // https://open.feishu.cn/document/client-docs/bot-v3/add-custom-bot
    public class FeiShuMessenger : ServerScript
    {
        public FeiShuMessenger(Server server) : base(server)
        {
            
        }

        async Task<string> Post(string url, string jsonData)
        {
            HttpClient httpClient = this.server.data.httpClient;

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var httpResponse = await httpClient.PostAsync(url, content);
            httpResponse.EnsureSuccessStatusCode();
            var readStr = await httpResponse.Content.ReadAsStringAsync();
            return readStr;
        }

        public async void SendErrorMessage(string title, string content)
        {
            ServerConfig.FeiShuConfig feiShuConfig = this.server.data.serverConfig.feiShuConfig;

            long nowS = TimeUtils.GetTimeS();
            List<long> fstimes = this.server.data.error_feiShuSentTimeS;

            while (fstimes.Count > 0 && fstimes[0] < nowS - feiShuConfig.error_limitTimeS)
            {
                fstimes.RemoveAt(0);
            }

            if (fstimes.Count >= feiShuConfig.error_limitCount)
            {
                Data.Program.LogInfo(string.Format("exceed, {0} > {1}", fstimes.Count, feiShuConfig.error_limitCount));
                return;
            }

            fstimes.Add(nowS);

            var json = JsonUtils.stringify(
                new
                {
                    msg_type = "text",
                    content = new
                    {
                        text = title + "\n" + content,
                    }
                });

            string ret = await this.Post(feiShuConfig.error_webhook, json);
        }

        public async void SendFatalMessage(string title, string content)
        {
            ServerConfig.FeiShuConfig feiShuConfig = this.server.data.serverConfig.feiShuConfig;

            long nowS = TimeUtils.GetTimeS();
            List<long> fstimes = this.server.data.fatal_feiShuSentTimeS;

            while (fstimes.Count > 0 && fstimes[0] < nowS - feiShuConfig.fatal_limitTimeS)
            {
                fstimes.RemoveAt(0);
            }

            if (fstimes.Count >= feiShuConfig.fatal_limitCount)
            {
                Data.Program.LogInfo(string.Format("exceed, {0} > {1}", fstimes.Count, feiShuConfig.fatal_limitCount));
                return;
            }

            fstimes.Add(nowS);

            var json = JsonUtils.stringify(
                new
                {
                    msg_type = "text",
                    content = new
                    {
                        text = title + "\n" + content,
                    }
                });

            string ret = await this.Post(feiShuConfig.fatal_webhook, json);
        }

        public void SendEventMessage(string formatter, params object[] objects)
        {
            try
            {
                string content = string.Format(formatter, objects);
                this.SendEventMessage(content);
            }
            catch (Exception ex)
            {
                Data.Program.LogInfo("SendEventMessageFormat exception! " + ex);
            }
        }

        public async void SendEventMessage(string content)
        {
            ServerConfig.FeiShuConfig feiShuConfig = this.server.data.serverConfig.feiShuConfig;
            if (!feiShuConfig.event_enabled || string.IsNullOrEmpty(feiShuConfig.event_webhook))
            {
                return;
            }

            content = "[" + this.server.data.serverConfig.purpose + "]" + content;

            var json = JsonUtils.stringify(
                new
                {
                    msg_type = "text",
                    content = new
                    {
                        text = content,
                    }
                });

            Exception? lastEx = null;

            for (int i = 0; i < 5; i++)
            {
                bool success = false;
                try
                {
                    string ret = await this.Post(feiShuConfig.event_webhook, json);
                    success = true;
                }
                catch (Exception ex)
                {
                    success = false;
                    lastEx = ex;
                }

                if (success)
                {
                    lastEx = null;
                    break;
                }

                await Task.Delay(1000);
            }

            if (lastEx != null)
            {
                Data.Program.LogInfo("SendEventMessage exception! " + lastEx);
            }
        }

        public async void SendChatMessage(string content)
        {
            try
            {
                ServerConfig.FeiShuConfig feiShuConfig = this.server.data.serverConfig.feiShuConfig;
                if (!feiShuConfig.chat_enabled || string.IsNullOrEmpty(feiShuConfig.chat_webhook))
                {
                    return;
                }

                content = "[" + this.server.data.serverConfig.purpose + "]" + content;

                var json = JsonUtils.stringify(
                    new
                    {
                        msg_type = "text",
                        content = new
                        {
                            text = content,
                        }
                    });

                string ret = await this.Post(feiShuConfig.chat_webhook, json);
            }
            catch (Exception ex)
            {
                Data.Program.LogInfo("SendChatMessage exception! " + ex);
            }
        }
    }
}