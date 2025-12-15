using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgSaveUserInfoToFile
    {
        [Key(0)]
        public long userId;
    }

    [MessagePackObject]
    public class ResSaveUserInfoToFile
    {
        [Key(0)]
        public string fileName;
    }
}