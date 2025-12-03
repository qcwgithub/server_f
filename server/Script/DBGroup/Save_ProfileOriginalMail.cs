using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_ProfileOriginalMail : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Save_ProfileOriginalMail;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_ProfileOriginalMail>(_msg);
            this.service.logger.InfoFormat("{0} mailId {1}", this.msgType, msg.info.mailId);

            ECode e = await this.service.collection_original_mail.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_ProfileOriginalMail();
            return new MyResponse(ECode.Success, res);
        }
    }
}
