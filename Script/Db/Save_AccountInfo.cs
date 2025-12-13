using Data;
using System.Threading.Tasks;

namespace Script
{
    //// AUTO CREATED ////
    public sealed class Save_AccountInfo : Handler<DbService, MsgSave_AccountInfo, ResSave_AccountInfo>
    {
        public override MsgType msgType => MsgType._Save_AccountInfo;

        public Save_AccountInfo(Server server, DbService service) : base(server, service)
        {
        }

        public override async Task<ECode> Handle(IConnection connection, MsgSave_AccountInfo msg, ResSave_AccountInfo res)
        {
            this.service.logger.InfoFormat("{0} channel {1} channelUserId {2}", this.msgType, msg.info.channel, msg.info.channelUserId);

            ECode e = await this.service.collection_account_info.Save(msg.info);
            if (e != ECode.Success)
            {
                return e;
            }

            return ECode.Success;
        }
    }
}
