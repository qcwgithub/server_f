using System.Net.Sockets;
using Data;

namespace Script
{
    public class TcpListenerScript : ServiceScript<Service>, ITcpListenerCallback
    {
        public TcpListenerScript(Server server, Service service) : base(server, service)
        {
        }


        public void LogError(string str)
        {
            this.service.logger.Error(str);
        }

        public void LogInfo(string str)
        {
            this.service.logger.Info(str);
        }

        /////////////////////// connect ///////////////////////////

        public string UrlForServer(string host, int port)
        {
            var url = //"http://" + 
            host + ":" + port;
            //url += "?sign=" + ServerConst.SERVER_SIGN;
            //url += "&purpose=" + this.server.purpose;
            return url;
        }

        void EnableKeepAlive(Socket socket)
        {
            /*
            // https://darchuk.net/2019/01/04/c-setting-socket-keep-alive/
            // Get the size of the uint to use to back the byte array
            int size = Marshal.SizeOf((uint)0);

            // Create the byte array
            byte[] keepAlive = new byte[size * 3];

            // Pack the byte array:
            // Turn keepalive on
            Buffer.BlockCopy(BitConverter.GetBytes((uint)1), 0, keepAlive, 0, size);
            // Set amount of time without activity before sending a keepalive to 5 seconds
            Buffer.BlockCopy(BitConverter.GetBytes((uint)5000), 0, keepAlive, size, size);
            // Set keepalive interval to 5 seconds
            Buffer.BlockCopy(BitConverter.GetBytes((uint)5000), 0, keepAlive, size * 2, size);

            // Set the keep-alive settings on the underlying Socket
            socket.IOControl(IOControlCode.KeepAliveValues, keepAlive, null);
            */

            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            // TcpKeepAliveInterval
            // 发送一个 probe 后，等多久再发送
            socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveInterval, 3);
            // TcpKeepAliveTime
            // 应该是，多久没动静了，才开始发送探针
            socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, 5);
            // TcpKeepAliveRetryCount
            // 总的尝试几次探针
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.TcpKeepAliveRetryCount, 2);
        }

        public void OnAcceptComplete(TcpListenerData tcpListener, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                // 有可能会走到这
                return;
            }

            var tcpClientData = new TcpClientData();
            Socket socket = e.AcceptSocket;
            socket.NoDelay = true;

            //////////////////////////////////////////////////////////
            // 先去掉，等客户端打包再测试
            // this.EnableKeepAlive(socket);
            //////////////////////////////////////////////////////////

            tcpClientData.AcceptorInit(this.service.data, socket, tcpListener.isForClient);

            // var msg = new MsgOnConnect { isAcceptor = tcpClientData.isAcceptor };
            // this.service.RawDispatch(tcpClientData, MsgType._OnSocketConnect, msg, null);

        }
    }
}