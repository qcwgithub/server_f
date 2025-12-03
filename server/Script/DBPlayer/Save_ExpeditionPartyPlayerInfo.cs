using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_ExpeditionPartyPlayerInfo : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Save_ExpeditionPartyPlayerInfo;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_ExpeditionPartyPlayerInfo>(_msg);
            this.service.logger.InfoFormat("{0} playerId {1}", this.msgType, msg.info.playerId);

            ECode e = await this.service.collection_expedition_party_player_info.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_ExpeditionPartyPlayerInfo();
            return new MyResponse(ECode.Success, res);
        }
    }
}
