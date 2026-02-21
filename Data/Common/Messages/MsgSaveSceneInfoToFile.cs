using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgSaveSceneInfoToFile
    {
        [Key(0)]
        public long roomId;
    }

    [MessagePackObject]
    public class ResSaveSceneInfoToFile
    {
        [Key(0)]
        public string fileName;
    }
}