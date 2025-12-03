using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_ExperiencePlayerInfo_by_playerId : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Query_ExperiencePlayerInfo_by_playerId;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_ExperiencePlayerInfo_by_playerId>(_msg);
            // this.service.logger.InfoFormat("{0} playerId: {1}", this.msgType, msg.playerId);

            var result = await this.service.collection_experience_player_info.Query_ExperiencePlayerInfo_by_playerId(msg.playerId);

            var res = new ResQuery_ExperiencePlayerInfo_by_playerId();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
