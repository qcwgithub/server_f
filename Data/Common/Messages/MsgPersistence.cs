using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgPersistence
    {
        [Key(0)]
        public bool isShuttingDownSaveAll;
    }
}