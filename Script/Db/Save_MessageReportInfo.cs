using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    [AutoRegister]
    public sealed class Save_MessageReportInfo : Handler<DbService, MsgSave_MessageReportInfo, ResSave_MessageReportInfo>
    {
        public override MsgType msgType => MsgType._Save_MessageReportInfo;

        public Save_MessageReportInfo(Server server, DbService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgSave_MessageReportInfo msg, ResSave_MessageReportInfo res)
        {
            this.service.logger.InfoFormat("{0}", this.msgType);

            ECode e = await this.service.collection_message_report_info.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            return ECode.Success;
        }
    }
}
