namespace Script
{
    public class MaxApexGroupIdRedis : GMaxIdRedis
    {
        public override string Key() => GGlobalKey.MaxApexGroupId();
    }
}