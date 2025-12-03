using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_ProfileGroupWorldBoss : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Save_ProfileGroupWorldBoss;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_ProfileGroupWorldBoss>(_msg);
            this.service.logger.InfoFormat("{0}", this.msgType);

            ECode e = await this.service.collection_profile_group_world_boss.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_ProfileGroupWorldBoss();
            return new MyResponse(ECode.Success, res);
        }
    }
}
