using Data;

namespace Script
{
    public class ChannelUuid : ServiceScript<UserManagerService>
    {
        public ChannelUuid(Server server, UserManagerService service) : base(server, service)
        {
            
        }

        public ECode VeryfyAccount(string platform, string channel, string channelUserId)
        {
            return ECode.Success;
        }
    }
}