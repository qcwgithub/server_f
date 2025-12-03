using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Query_ChampionInfo_all : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Query_ChampionInfo_all;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgQuery_ChampionInfo_all>(_msg);
            // this.service.logger.InfoFormat("{0}", this.msgType);

            var result = await this.service.collection_champion_info.Query_ChampionInfo_all();

            var res = new ResQuery_ChampionInfo_all();
            res.result = result;
            return new MyResponse(ECode.Success, res);
        }
    }
}
