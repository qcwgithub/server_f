using System;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public static class MyResponseExtension
    {
        public static Task<MyResponse> ToTask(this MyResponse @this)
        {
            return Task.FromResult<MyResponse>(@this);
        }
        public static Task<MyResponse> ToTask(this ECode e)
        {
            return Task.FromResult<MyResponse>(new MyResponse(e, null));
        }
        public static Task<ECode> ToTaskE(this ECode e)
        {
            return Task.FromResult<ECode>(e);
        }
    }
}