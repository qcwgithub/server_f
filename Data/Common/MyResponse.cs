namespace Data
{
    public abstract class MyResponse
    {
        public MyResponse(ECode e)
        {
            this.e = e;
        }
        public ECode e;
    }
}