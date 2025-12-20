namespace Data
{
    public class UserServiceAllocatorData
    {
        public long lastUpdateS;
        public readonly Dictionary<int, UserServiceInfo> userServiceInfoDict;
        public UserServiceAllocatorData()
        {
            this.userServiceInfoDict = new Dictionary<int, UserServiceInfo>();
        }

        public void Update(long timeS, Dictionary<int, UserServiceInfo> dict)
        {
            this.userServiceInfoDict.Clear();
            foreach (var kv in dict)
            {
                this.userServiceInfoDict[kv.Key] = kv.Value;
            }
        }
    }
}