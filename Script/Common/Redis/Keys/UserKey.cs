namespace Script
{
    public static class UserKey
    {
        public static string Location(long userId) => "user:" + userId + ":location";
        public static string BriefInfo(long userId) => "user:" + userId + ":brief";
        public static string FriendChatInBox(long userId) => "user:" + userId + ":friendChatInBox";
        public static string FriendChatState(long userId) => "user:" + userId + ":friendChatState";
    }
}