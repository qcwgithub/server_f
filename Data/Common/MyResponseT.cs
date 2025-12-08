namespace Data
{
    public class MyResponse<Res> : MyResponse where Res : class
    {
        public MyResponse(ECode e, Res r) : base(e)
        {
            this.res = r;
        }
        public Res res;
        public int flags;
        public const int DONT_LOG_ERROR = 1;
        public MyResponse<Res> DontLogError()
        {
            this.flags |= DONT_LOG_ERROR;
            return this;
        }
        public MyResponse<Res> DontLogError(params ECode[] es)
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
    }
}