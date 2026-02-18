public enum CacheType
{
    None,
    Memory,
    RedisJson,
    MemoryChild,
    RedisBinary,
}

public static class CacheTypeExt
{
    public static bool IsRedis(this CacheType e)
    {
        return e == CacheType.RedisJson || e == CacheType.RedisBinary;
    }

    public static bool IsCreateProxy(this CacheType e)
    {
        return IsRedis(e);
    }

    public static bool IsCreatePersistence(this CacheType e)
    {
        return IsRedis(e);
    }
}