using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_ExpeditionInfo : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Save_ExpeditionInfo;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_ExpeditionInfo>(_msg);
            this.service.logger.InfoFormat("{0}", this.msgType);

            ECode e = await this.service.collection_expedition_info.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_ExpeditionInfo();
            return new MyResponse(ECode.Success, res);
        }
    }
}
