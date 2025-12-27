namespace Data
{
    public class ObjectLocationAssignmentData
    {
        public long lastUpdateS;
        public readonly Dictionary<int, ServiceRuntimeInfo> serviceRuntimeInfoDict;
        public ObjectLocationAssignmentData()
        {
            this.serviceRuntimeInfoDict = new Dictionary<int, ServiceRuntimeInfo>();
        }

        public void Update(long timeS, Dictionary<int, ServiceRuntimeInfo> dict)
        {
            this.lastUpdateS = timeS;

            this.serviceRuntimeInfoDict.Clear();
            foreach (var kv in dict)
            {
                this.serviceRuntimeInfoDict[kv.Key] = kv.Value;
            }
        }
    }
}