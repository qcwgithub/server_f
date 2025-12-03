using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Iterate_listOf_ProfileOriginalMail_by_mailId_nd : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Iterate_listOf_ProfileOriginalMail_by_mailId_nd;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgIterate_listOf_ProfileOriginalMail_by_mailId_nd>(_msg);
            // this.service.logger.InfoFormat("{0} start_mailId: {1}, end_mailId: {2}", this.msgType, msg.start_mailId, msg.end_mailId);

            var result = await this.service.collection_original_mail.Iterate_listOf_ProfileOriginalMail_by_mailId_nd(msg.start_mailId, msg.end_mailId);

            var res = new ResIterate_listOf_ProfileOriginalMail_by_mailId_nd();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
