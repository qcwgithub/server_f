using System.Threading.Tasks;
using Data;

namespace Script
{
    public class OnReloadConfigs<S> : Handler<S>
        where S : Service
    {
        public override MsgType msgType => MsgType._ReloadConfigs;

        public override Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgReloadConfigs>(_msg);

            string message = $"[{this.service.serviceId}]{this.msgType} all? {msg.all} files? {JsonUtils.stringify(msg.files)}";
            this.service.logger.Info(message);
            this.server.feiShuMessenger.SendEventMessage(message);

            if (!msg.all)
            {
                if (msg.files == null || msg.files.Count == 0)
                {
                    return ECode.InvalidParam.ToTask();
                }
            }

            this.service.data.ReloadConfigs(msg.all, msg.files);

            var res = new ResReloadConfigs();
            return new MyResponse(ECode.Success, res).ToTask();
        }
    }
}