using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_ChampionInfo : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Save_ChampionInfo;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_ChampionInfo>(_msg);
            this.service.logger.InfoFormat("{0}", this.msgType);

            ECode e = await this.service.collection_champion_info.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_ChampionInfo();
            return new MyResponse(ECode.Success, res);
        }
    }
}
