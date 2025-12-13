using Data;

namespace Script
{
    // Lock 后必须先 Unlock 才可以再次 Lock
    public abstract class LockManuallyHandler<S, Msg, Res> : Handler<S, Msg, Res>
        where S : Service
        where Msg : class
        where Res : class, new()
    {
        protected LockManuallyHandler(Server server, S service) : base(server, service)
        {
        }

        protected abstract LockController lockController { get; }

        protected async Task<ECode> Lock(LockOptionsManually options)
        {
            if (options.locked)
            {
                throw new Exception();
            }

            // *Lock*
            if (!options.IsEmpty())
            {
                bool success = await this.lockController.Lock(options.GetKeys(), options.lockTimeS, options.retry);
                if (!success)
                {
                    return ECode.ServerBusy;
                }
                options.locked = true;
                options.startS = TimeUtils.GetTimeS();
            }
            return ECode.Success;
        }

        protected async Task Unlock(LockOptionsManually options)
        {
            // *Unlock*
            if (!options.IsEmpty() && options.locked)
            {
                await this.lockController.Unlock(options.GetKeys());
                options.locked = false;
                lockController.DetectLockTooLong(this.msgType, options.startS, options.lockTimeS);
            }
        }
    }
}