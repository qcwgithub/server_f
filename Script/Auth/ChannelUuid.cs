using Data;

namespace Script
{
    public class ChannelUuid : ServiceScript<AuthService>
    {
        public ChannelUuid(Server server, AuthService service) : base(server, service)
        {
            
        }

        public ECode Auth(MsgUserLogin msg)
        {
            return ECode.Success;
        }
    }
}