using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgSaveRoomInfoToFile
    {
        [Key(0)]
        public long roomId;
    }

    [MessagePackObject]
    public class ResSaveRoomInfoToFile
    {
        [Key(0)]
        public string fileName;
    }
}