namespace Data
{
    public static class UserBriefInfoExt
    {
        public static UserBriefInfo CreateDefaultForRedis(this UserBriefInfo self, long userId)
        {
            self.userId = userId;
            self.userName = "User_" + userId;
            self.avatarIndex = 0;
            return self;
        }
    }
}