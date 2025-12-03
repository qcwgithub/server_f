using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgSaveProfileToFile
    {
        [Key(0)]
        public long userId;
    }

    [MessagePackObject]
    public class ResSaveProfileToFile
    {
        [Key(0)]
        public string fileName;
    }
}