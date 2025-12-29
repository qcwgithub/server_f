using Data;

namespace Script
{
    public abstract class LockInAdvanceHandler<S, Msg, Res> : Handler<S, Msg, Res>
        where S : Service
        where Msg : class
        where Res : class, new()
    {
        public LockInAdvanceHandler(Server server, S service) : base(server, service)
        {
            
        }

        protected abstract LockController lockController { get; }
        protected virtual LockController GetLockController(Msg msg)
        {
            return null;
        }

        public sealed override async Task<ECode> Handle(MsgContext context, Msg msg, Res res)
        {
            ECode e = ECode.Success;
            if (this.hasBeforeLock)
            {
                e = await this.BeforeLock(msg);
                if (e != ECode.Success)
                {
                    return e;
                }
            }

            var options = new LockOptions();
            options.lockTimeS = 10;
            options.retry = true;

            e = await this.PrepareLockOptions(msg, options);
            if (e != ECode.Success)
            {
                return e;
            }

            LockController lc = this.lockController;
            if (lc == null)
            {
                lc = this.GetLockController(msg);
            }

            // *Lock*
            if (!options.IsEmpty())
            {
                bool success = await lc.Lock(options.GetKeys(), options.lockTimeS, options.retry);
                if (!success)
                {
                    return ECode.ServerBusy;
                }
                options.startS = TimeUtils.GetTimeS();
            }

            try
            {
                e = await this.HandleDuringLocked(msg, res);
            }
            catch (Exception ex)
            {
                this.service.logger.Error("exception msgType: " + this.msgType, ex);
                e = ECode.Exception;
            }
            finally
            {
                // *Unlock*
                if (!options.IsEmpty())
                {
                    await lc.Unlock(options.GetKeys());
                    lc.DetectLockTooLong(this.msgType, options.startS, options.lockTimeS);
                }
            }

            return e;
        }

        protected virtual bool hasBeforeLock => false;
        protected virtual Task<ECode> BeforeLock(Msg msg)
        {
            throw new NotImplementedException();
        }

        protected virtual Task<ECode> PrepareLockOptions(Msg msg, LockOptions options)
        {
            throw new NotImplementedException();
        }
        protected abstract Task<ECode> HandleDuringLocked(Msg msg, Res res);
    }
}