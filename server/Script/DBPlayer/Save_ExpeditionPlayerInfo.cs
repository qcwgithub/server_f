using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_ExpeditionPlayerInfo : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Save_ExpeditionPlayerInfo;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_ExpeditionPlayerInfo>(_msg);
            this.service.logger.InfoFormat("{0} playerId {1}", this.msgType, msg.info.playerId);

            ECode e = await this.service.collection_expedition_player_info.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_ExpeditionPlayerInfo();
            return new MyResponse(ECode.Success, res);
        }
    }
}
