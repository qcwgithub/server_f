namespace Script
{
    public class MaxUnionMatchIdRedis : GMaxIdRedis
    {
        public override string Key() => GGlobalKey.MaxUnionMatchId();
    }
}