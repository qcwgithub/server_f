namespace Script
{
    public static class UserKey
    {
        public static string OwningServiceId(long userId) => "user:" + userId + ":owningServiceId";
    }
}