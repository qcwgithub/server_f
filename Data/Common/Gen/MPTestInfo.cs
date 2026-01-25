using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MPTestInfo
    {
        #region auto

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

        public static MPTestInfo Ensure(MPTestInfo? p)
        {
            if (p == null)
            {
                p = new MPTestInfo();
            }
            p.Ensure();
            return p;
        }

        public void Ensure()
        {
            if (this.stringValue == null)
            {
                this.stringValue = string.Empty;
            }
            if (this.listOfInt == null)
            {
                this.listOfInt = new List<int>();
            }
            if (this.listOfLong == null)
            {
                this.listOfLong = new List<long>();
            }
            if (this.listOfString == null)
            {
                this.listOfString = new List<string>();
            }
        }

        public bool IsDifferent(MPTestInfo other)
        {
            if (this.intValue != other.intValue)
            {
                return true;
            }
            if (this.boolValue != other.boolValue)
            {
                return true;
            }
            if (this.longValue != other.longValue)
            {
                return true;
            }
            if (this.stringValue != other.stringValue)
            {
                return true;
            }
            if (this.listOfInt.IsDifferent_ListValue(other.listOfInt))
            {
                return true;
            }
            if (this.listOfLong.IsDifferent_ListValue(other.listOfLong))
            {
                return true;
            }
            if (this.listOfString.IsDifferent_ListValue(other.listOfString))
            {
                return true;
            }
            return false;
        }

        public void DeepCopyFrom(MPTestInfo other)
        {
            this.intValue = other.intValue;
            this.boolValue = other.boolValue;
            this.longValue = other.longValue;
            this.stringValue = other.stringValue;
            this.listOfInt.DeepCopyFrom_ListValue(other.listOfInt);
            this.listOfLong.DeepCopyFrom_ListValue(other.listOfLong);
            this.listOfString.DeepCopyFrom_ListValue(other.listOfString);
        }

        #endregion auto
    }
}