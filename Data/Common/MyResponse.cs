namespace Data
{
    public class MyResponse
    {
        public MyResponse(ECode e, object? r = null)
        {
            this.err = e;
            this.res = r;
        }
        public ECode err;
        public object? res;
        public int flags;
        public const int DONT_LOG_ERROR = 1;
        public MyResponse DontLogError()
        {
            this.flags |= DONT_LOG_ERROR;
            return this;
        }
        public MyResponse DontLogError(params ECode[] es)
        {
            foreach (ECode e in es)
            {
                if (e == this.err)
                {
                    return this.DontLogError();
                }
            }
            return this;
        }
        public bool HasDontLogErrorFlag()
        {
            return (this.flags & DONT_LOG_ERROR) != 0;
        }

        public static implicit operator MyResponse(ECode e) => new MyResponse(e);

        public static implicit operator ECode(MyResponse v)
        {
            throw new NotImplementedException();
        }

        public T CastRes<T>() where T : class
        {
            if (this.res == null)
            {
                return null;
            }

            T? t = this.res as T;
            if (t == null)
            {
                throw new InvalidCastException(string.Format("CastRes typeof(this.res)={0}, should be {1}", this.res.GetType().Name, typeof(T).Name));
            }
            return t;
        }
    }
}