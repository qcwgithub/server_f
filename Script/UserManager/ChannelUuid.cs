using Data;

namespace Script
{
    public class ChannelUuid : ServiceScript<UserManagerService>
    {
        public ChannelUuid(Server server, UserManagerService service) : base(server, service)
        {
            
        }

        public ECode Auth(MsgUserLogin msg)
        {
            return ECode.Success;
        }
    }
}