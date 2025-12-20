namespace Script
{
    public static class AccountKey
    {
        public static string AccountInfo(string channel, string channelUserId) => "account:" + channel + ":" + channelUserId;
    }
}