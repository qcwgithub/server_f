using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class ChatMessageImageContent
    {
        [Key(0)]
        public string url;
        [Key(1)]
        public int width;
        [Key(2)]
        public int height;
        [Key(3)]
        public long size;
        [Key(4)]
        public string thumbnailUrl;
    }
}
