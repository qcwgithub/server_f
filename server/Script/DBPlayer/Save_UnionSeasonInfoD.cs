using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_UnionSeasonInfoD : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Save_UnionSeasonInfoD;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_UnionSeasonInfoD>(_msg);
            this.service.logger.InfoFormat("{0} unionId {1}", this.msgType, msg.info.unionId);

            ECode e = await this.service.collection_union_season_info_d.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_UnionSeasonInfoD();
            return new MyResponse(ECode.Success, res);
        }
    }
}
