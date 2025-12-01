using System.Text;
using System.Collections.Generic;
using System.Net;
using System;
using System.Net.Http;
using System.Linq;

namespace Data
{
    public class HttpListenerData
    {
        public HttpListener httpListener;
        public IHttpListenerCallbackProvider callbackProvider;
        public IHttpListenerCallback callback => this.callbackProvider.GetHttpListenerCallback();
        public log4net.ILog logger => this.callback.GetLogger();
        List<IPAddress> allowedIpAddresses = new List<IPAddress>();
        public void Listen(string[] prefixes)
        {
            if (prefixes == null || prefixes.Length == 0)
            {
                throw new Exception("HttpListenerData.Listen(): prefixes == null || prefixes.Length == 0");
            }

            this.httpListener = new HttpListener();
            foreach (string prefix in prefixes)
            {
                this.httpListener.Prefixes.Add(prefix);
            }

            // this.httpListener.AuthenticationSchemeSelectorDelegate =
            //     new AuthenticationSchemeSelector(this.AuthenticationSchemeForClient);

            this.httpListener.Start();
        }

        public void SetAllowedIps(IEnumerable<string> allowedIps)
        {
            this.allowedIpAddresses.Clear();
            if (allowedIps != null)
            {
                foreach (string ip in allowedIps)
                {
                    this.allowedIpAddresses.Add(IPAddress.Parse(ip));
                }
            }
        }

#if DEBUG
        bool stopping;
        public async void Stop()
#else
        public void Stop()

#endif
        {
#if DEBUG
            this.stopping = true;
            using (var client = new HttpClient())
            {
                string prefix0 = this.httpListener.Prefixes.First().ToString();
                int index = prefix0.LastIndexOf(':');
                int count = 0;
                for (int i = index + 1; i < prefix0.Length; i++)
                {
                    if (char.IsDigit(prefix0[i]))
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
                int port = int.Parse(prefix0.Substring(index + 1, count));
                await client.GetAsync("http://localhost:" + port);
            }
#endif

            this.httpListener.Stop();
            this.httpListener = null;
        }

        // AuthenticationSchemes AuthenticationSchemeForClient(HttpListenerRequest request)
        // {
        //     if (this.allowedIpAddresses.Count == 0)
        //     {
        //         return this.httpListener.AuthenticationSchemes;
        //     }

        //     foreach (IPAddress addr in this.allowedIpAddresses)
        //     {
        //         if (request.RemoteEndPoint.Address.Equals(addr))
        //         {
        //             return this.httpListener.AuthenticationSchemes;
        //         }
        //     }

        //     this.logger.ErrorFormat("received from black ip {0}", request.RemoteEndPoint.Address);
        //     return AuthenticationSchemes.None;
        // }

        public async void Accept()
        {
            while (true)
            {
                HttpListenerContext context = null;

                try
                {
                    context = await this.httpListener.GetContextAsync();
#if DEBUG
                if (this.stopping)
                {
                    break;
                }
#endif
                }
                catch (Exception ex)
                {
                    if (this.httpListener == null)
                    {
                        // 已经 Stop 了，不是错误
                        break;
                    }
                    else
                    {
                        this.logger.Error("this.httpListener.GetContextAsync exception: " + ex);
                        continue;
                    }
                }
                // HttpListenerRequest request = context.Request;
                // HttpListenerResponse response = context.Response;


                int place = 0;
                try
                {
                    if (this.allowedIpAddresses.Count > 0)
                    {
                        place = 1;

                        HttpListenerRequest request = context.Request;
                        place = 2;

                        if (request == null)
                        {
                            this.logger.Error("request == null");
                        }
                        place = 3;

                        if (request.RemoteEndPoint == null)
                        {
                            this.logger.Error("request.RemoteEndPoint == null");
                        }
                        place = 4;

                        if (request.RemoteEndPoint.Address == null)
                        {
                            this.logger.Error("request.RemoteEndPoint.Address == null");
                        }
                        place = 5;

                        bool ok = false;
                        foreach (IPAddress addr in this.allowedIpAddresses)
                        {
                            if (request.RemoteEndPoint.Address.Equals(addr))
                            {
                                ok = true;
                                break;
                            }
                        }
                        place = 6;

                        if (!ok)
                        {
                            this.logger.InfoFormat("received from black ip {0}", request.RemoteEndPoint.Address);
                            place = 8;

                            HttpListenerResponse response = context.Response;
                            if (response == null)
                            {
                                this.logger.Error("response== null");
                            }
                            place = 9;

                            response.StatusCode = 403;
                            place = 10;

                            response.Close();
                            place = 11;

                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.logger.Error("HttpListenerData.Accept exception 1, place " + place, ex);
                    continue;
                }

                try
                {
                    this.callback.OnReceiveHttpRequest(context);
                }
                catch (Exception ex)
                {
                    this.logger.Error("HttpListenerData.Accept exception 2", ex);
                }
            }
        }
    }
}