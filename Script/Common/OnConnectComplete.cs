using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;

namespace Script
{
    // 连接其他服务器成功
    public class OnConnectComplete<S> : Handler<S, MsgConnectorInfo, ResConnectorInfo>
        where S : Service
    {
        public OnConnectComplete(Server server, S service) : base(server, service)
        {
        }


        public override MsgType msgType => MsgType._OnConnectComplete;

        public override async Task<ECode> Handle(IConnection connection, MsgConnectorInfo msg, ResConnectorInfo res)
        {
            this.logger.InfoFormat("{0} connection id: {1}, to: {2}", this.msgType, connection.GetSocketId(), connection.serviceTypeAndId.Value.ToString());

            // 连上去之后立即向他报告是我的身份
            var msgInfo = new MsgConnectorInfo();
            msgInfo.connectorInfo = this.service.CreateConnectorInfo();
            await connection.Request<MsgConnectorInfo, ResConnectorInfo>(MsgType._ConnectorInfo, msgInfo);

            // var s = connection;
            // bool isClient = !msg.isServer;
            return ECode.Success;
        }
    }
}