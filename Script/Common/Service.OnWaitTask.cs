using System.Threading.Tasks;
using Data;

namespace Script
{
    public partial class Service
    {
        public async Task<ECode> WaitTask(Task task)
        {
            await task;
            return ECode.Success;
        }
    }
}