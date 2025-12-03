using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_ProfileWorldBoss : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Save_ProfileWorldBoss;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_ProfileWorldBoss>(_msg);
            this.service.logger.InfoFormat("{0}", this.msgType);

            ECode e = await this.service.collection_profile_world_boss.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_ProfileWorldBoss();
            return new MyResponse(ECode.Success, res);
        }
    }
}
