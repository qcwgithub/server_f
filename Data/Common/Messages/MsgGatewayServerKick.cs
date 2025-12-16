using MessagePack;


namespace Data
{
    [MessagePackObject]
    public class MsgGatewayServerKick
    {
        [Key(0)]
        public long userId;
        [Key(1)]
        public bool logoutSdk;
    }

    [MessagePackObject]
    public class ResGatewayServerKick
    {

    }
}