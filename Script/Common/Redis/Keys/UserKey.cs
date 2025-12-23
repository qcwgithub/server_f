namespace Script
{
    public static class UserKey
    {
        public static string Brief(long userId) => "user:" + userId + ":brief";

        public static string USId(long userId) => "user:" + userId + ":usId";
    }
}