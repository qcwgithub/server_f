namespace Data
{
    public class RoomUser
    {
        public long userId;
        public int gatewayServiceId;
        public long lastSendChatStamp;
        public readonly List<long> sendChatTimestamps = new();
    }
}