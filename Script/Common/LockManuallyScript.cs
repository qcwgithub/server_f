using Data;

namespace Script
{
    // Lock 后必须先 Unlock 才可以再次 Lock
    public class LockManuallyScript : ServiceScript<Service>
    {
        public LockManuallyScript(Server server, Service service) : base(server, service)
        {
        }

        public async Task<ECode> Lock(LockController lockController, LockOptionsManually options)
        {
            if (options.locked)
            {
                throw new Exception();
            }

            // *Lock*
            if (!options.IsEmpty())
            {
                bool success = await lockController.Lock(options.GetKeys(), options.lockTimeS, options.retry);
                if (!success)
                {
                    return ECode.ServerBusy;
                }
                options.locked = true;
                options.startS = TimeUtils.GetTimeS();
            }
            return ECode.Success;
        }

        public async Task Unlock(LockController lockController, LockOptionsManually options, string who)
        {
            // *Unlock*
            if (!options.IsEmpty() && options.locked)
            {
                await lockController.Unlock(options.GetKeys());
                options.locked = false;
                lockController.DetectLockTooLong(who, options.startS, options.lockTimeS);
            }
        }
    }
}