using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgSaveSceneRoomInfoToFile
    {
        [Key(0)]
        public long roomId;
    }

    [MessagePackObject]
    public class ResSaveSceneRoomInfoToFile
    {
        [Key(0)]
        public string fileName;
    }
}