using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_ProfileOriginalMail_by_mailId : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Query_ProfileOriginalMail_by_mailId;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_ProfileOriginalMail_by_mailId>(_msg);
            // this.service.logger.InfoFormat("{0} mailId: {1}", this.msgType, msg.mailId);

            var result = await this.service.collection_original_mail.Query_ProfileOriginalMail_by_mailId(msg.mailId);

            var res = new ResQuery_ProfileOriginalMail_by_mailId();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
