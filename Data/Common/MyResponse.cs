namespace Data
{
    public class MyResponse
    {
        public MyResponse(ECode e)
        {
            this.e = e;
        }
        public MyResponse(ECode e, object res)
        {
            this.e = e;
            this.res = res;
        }
        public ECode e;
        public object res;
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
                if (e == this.e)
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
        public T CastRes<T>() where T : class
        {
            T? t = this.res as T;
            if (t == null)
            {
                throw new System.InvalidCastException(string.Format("CastRes typeof(this.res)={0}, should be {1}", this.res.GetType().Name, typeof(T).Name));
            }
            return t;
        }
    }
}