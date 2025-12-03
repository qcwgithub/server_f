using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_ProfileExpeditionParty : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Save_ProfileExpeditionParty;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_ProfileExpeditionParty>(_msg);
            this.service.logger.InfoFormat("{0}", this.msgType);

            ECode e = await this.service.collection_profile_expedition_party.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_ProfileExpeditionParty();
            return new MyResponse(ECode.Success, res);
        }
    }
}
