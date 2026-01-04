using Data;

namespace Script
{
    public partial class RobotService
    {
        public override async Task<ECode> OnConnectComplete(IConnection connection)
        {
            var e = await base.OnConnectComplete(connection);
            if (e != ECode.Success)
            {
                return e;
            }

            if (connection is RobotClientConnection robotClientConnection)
            {
                await Task.Delay(1000);

                var msg = new MsgLogin();
                msg.version = string.Empty;
                msg.platform = "Windows";
                msg.channel = MyChannels.uuid;
                msg.channelUserId = "1000";

                var r = await robotClientConnection.Request(MsgType.Login, msg);
                if (r.e == ECode.Success)
                {
                    var res = r.CastRes<ResLogin>();
                    //
                }
            }
            return e;
        }
    }
}