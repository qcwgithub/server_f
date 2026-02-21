namespace Data
{
    public abstract class Room
    {
        public RoomType roomType { get; protected set; }
        public long roomId { get; protected set; }

        public ITimer? destroyTimer;
        public ITimer? saveTimer;

        public abstract void OnAddedToDict();


        public readonly Dictionary<long, long> lastSendChatStampDict = new();
        public readonly Dictionary<long, List<long>> sendChatTimestampsDict = new();
    }
}