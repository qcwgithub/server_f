using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_UserReportInfo : Handler<DbService, MsgSave_UserReportInfo, ResSave_UserReportInfo>
    {
        public override MsgType msgType => MsgType._Save_UserReportInfo;

        public Save_UserReportInfo(Server server, DbService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgSave_UserReportInfo msg, ResSave_UserReportInfo res)
        {
            this.service.logger.InfoFormat("{0}", this.msgType);

            ECode e = await this.service.collection_user_report_info.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            return ECode.Success;
        }
    }
}
