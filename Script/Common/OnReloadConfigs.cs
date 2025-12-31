using System.Threading.Tasks;
using Data;

namespace Script
{
    public class OnReloadConfigs<S> : Handler<S, MsgReloadConfigs, ResReloadConfigs>
        where S : Service
    {
        public OnReloadConfigs(Server server, S service) : base(server, service)
        {
        }

        public override MsgType msgType => MsgType._Service_ReloadConfigs;

        public override async Task<ECode> Handle(MessageContext context, MsgReloadConfigs msg, ResReloadConfigs res)
        {
            string message = $"[{this.service.serviceId}]{this.msgType} all? {msg.all} files? {JsonUtils.stringify(msg.files)}";
            this.service.logger.Info(message);
            this.server.feiShuMessenger.SendEventMessage(message);

            if (!msg.all)
            {
                if (msg.files == null || msg.files.Count == 0)
                {
                    return ECode.InvalidParam;
                }
            }

            this.service.data.ReloadConfigs(msg.all, msg.files);
            return ECode.Success;
        }
    }
}