using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgRoomManagerImportRoomConfig
    {
        [Key(0)]
        public string file;
    }

    [MessagePackObject]
    public class ResRoomManagerImportRoomConfig
    {

    }
}