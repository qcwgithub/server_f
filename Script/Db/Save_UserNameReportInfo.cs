using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_UserNameReportInfo : Handler<DbService, MsgSave_UserNameReportInfo, ResSave_UserNameReportInfo>
    {
        public override MsgType msgType => MsgType._Save_UserNameReportInfo;

        public Save_UserNameReportInfo(Server server, DbService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgSave_UserNameReportInfo msg, ResSave_UserNameReportInfo res)
        {
            this.service.logger.InfoFormat("{0}", this.msgType);

            ECode e = await this.service.collection_user_name_report_info.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            return ECode.Success;
        }
    }
}
