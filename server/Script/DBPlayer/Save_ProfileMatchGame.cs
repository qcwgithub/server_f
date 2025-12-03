using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_ProfileMatchGame : Handler<NormalServer, DBPlayerService>
    {
        public override MsgType msgType => MsgType._Save_ProfileMatchGame;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_ProfileMatchGame>(_msg);
            this.service.logger.InfoFormat("{0}", this.msgType);

            ECode e = await this.service.collection_profile_match_game.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_ProfileMatchGame();
            return new MyResponse(ECode.Success, res);
        }
    }
}
