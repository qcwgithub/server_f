using Data;
using System.Threading.Tasks;

using ChampionPlayerBattleSide = Data.PlayerBattleSide;
using ChampionPlayerBattleSide_Db = Data.PlayerBattleSide_Db;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_ChampionPlayerBattleSide : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Save_ChampionPlayerBattleSide;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_ChampionPlayerBattleSide>(_msg);
            this.service.logger.InfoFormat("{0} playerId {1}", this.msgType, msg.info.playerId);

            ECode e = await this.service.collection_champion_player_battle_side.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_ChampionPlayerBattleSide();
            return new MyResponse(ECode.Success, res);
        }
    }
}
