using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgUserDisconnectFromGateway
    {
        [Key(0)]
        public long userId;
    }

    [MessagePackObject]
    public class ResUserDisconnectFromGateway
    {
        
    }
}