using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_RoomMessageReportInfo : Handler<DbService, MsgSave_RoomMessageReportInfo, ResSave_RoomMessageReportInfo>
    {
        public override MsgType msgType => MsgType._Save_RoomMessageReportInfo;

        public Save_RoomMessageReportInfo(Server server, DbService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(MessageContext context, MsgSave_RoomMessageReportInfo msg, ResSave_RoomMessageReportInfo res)
        {
            this.service.logger.InfoFormat("{0}", this.msgType);

            ECode e = await this.service.collection_room_message_report_info.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            return ECode.Success;
        }
    }
}
