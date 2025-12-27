namespace Script
{
    public static class UserKey
    {
        public static string Location(long userId) => "user:" + userId + ":location";
    }
}