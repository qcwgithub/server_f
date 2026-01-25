public enum CacheType
{
    None,
    Memory,
    Redis,
}

public static class CacheTypeExt
{
    public static bool IsCreateProxy(this CacheType e)
    {
        return e == CacheType.Redis;
    }
    public static bool IsCreatePersistence(this CacheType e)
    {
        return e == CacheType.Redis;
    }
}