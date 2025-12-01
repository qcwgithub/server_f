namespace Data
{
    public class MyResponse
    {
        public MyResponse(ECode e, object r = null)
        {
            this.err = e;
            this.res = r;
        }
        public ECode err;
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

        public static MyResponse noResponse = new MyResponse(ECode.NoResponse);
        public static MyResponse exResponse = new MyResponse(ECode.Exception);
        public static MyResponse badReturnResponse = new MyResponse(ECode.BadReturn);

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

            T t = this.res as T;
            if (t == null)
            {
#if UNITY_2017_1_OR_NEWER
                UnityEngine.Debug.LogErrorFormat(string.Format("CastRes typeof(this.res)={0}, should be {1}", this.res.GetType().Name, typeof(T).Name));
#else
                throw new System.InvalidCastException(string.Format("CastRes typeof(this.res)={0}, should be {1}", this.res.GetType().Name, typeof(T).Name));
#endif
            }
            return t;
        }

        // public MyResponse Combine(object _res)
        // {
        //     var list = this.res as List<object>;
        //     if (list == null)
        //     {
        //         list = new List<object>();
        //         list.Add(this.res);
        //         this.res = list;
        //     }

        //     if (_res is List<object> _list)
        //     {
        //         list.AddRange(_list);
        //     }
        //     else
        //     {
        //         list.Add(_res);
        //     }
        //     return this;
        // }
    }
}