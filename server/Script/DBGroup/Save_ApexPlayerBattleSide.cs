using Data;
using System.Threading.Tasks;

using ApexPlayerBattleSide = Data.PlayerBattleSide;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_ApexPlayerBattleSide : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Save_ApexPlayerBattleSide;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_ApexPlayerBattleSide>(_msg);
            this.service.logger.InfoFormat("{0} playerId {1}", this.msgType, msg.info.playerId);

            ECode e = await this.service.collection_apex_player_battle_side.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_ApexPlayerBattleSide();
            return new MyResponse(ECode.Success, res);
        }
    }
}
