using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data
{
    public class LockRequest
    {
        public string[] keys;
        public int lockTimeS;
        public bool retry;
        public TaskCompletionSource<bool> taskCompetitionSource;

        // DEBUG
        // public long time0;
        // public int tryCount;
    }

    public class LockControllerData
    {
        public bool procedureBusy;
        public List<LockRequest> requests = new List<LockRequest>();
    }
}