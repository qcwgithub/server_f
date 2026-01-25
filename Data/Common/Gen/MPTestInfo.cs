using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MPTestInfo
    {
        [Key(0)]
        public int intValue;
        [Key(1)]
        public bool boolValue;
        [Key(2)]
        public long longValue;
        [Key(3)]
        public string stringValue;
        [Key(4)]
        public List<int> listOfInt;
        [Key(5)]
        public List<long> listOfLong;
        [Key(6)]
        public List<string> listOfString;
    }
}
