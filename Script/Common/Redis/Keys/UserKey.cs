namespace Script
{
    public static class UserKey
    {
        public static string Brief(long userId) => "user:" + userId + ":brief";

        public static string PSId(long userId) => "user:" + userId + ":psId";
    }
}