using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_GroupChampionInfo : Handler<GroupServer, DBGroupService>
    {
        public override MsgType msgType => MsgType._Save_GroupChampionInfo;

        public override async Task<MyResponse> Handle(ProtocolClientData socket, object _msg)
        {
            var msg = Utils.CastObject<MsgSave_GroupChampionInfo>(_msg);
            this.service.logger.InfoFormat("{0}", this.msgType);

            ECode e = await this.service.collection_group_champion_info.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            var res = new ResSave_GroupChampionInfo();
            return new MyResponse(ECode.Success, res);
        }
    }
}
