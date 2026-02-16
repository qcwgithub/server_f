using System;
using MessagePack;


namespace Data
{
    public static class DirtyElementManual
    {
        
    }

    [MessagePackObject]
    public struct stDirtyElement // 不可与 DirtyElementManual 中重复
    {
        [Key(0)]
        public DirtyElementType e;
        [Key(1)]
        public string s1;
        [Key(2)]
        public string s2;

        #region auto_create

        public static stDirtyElement Create_AccountInfo(string channel, string channelUserId)
        {
            return new stDirtyElement { e = DirtyElementType.AccountInfo, s1 = channel, s2 = channelUserId };
        }
        public static stDirtyElement Create_UserBriefInfo(long userId)
        {
            return new stDirtyElement { e = DirtyElementType.UserBriefInfo, s1 = userId.ToString() };
        }

        #endregion auto_create

        public const char SPLITER = '@';

        public override string ToString()
        {
            switch (this.e)
            {
                #region auto_toString

                case DirtyElementType.AccountInfo:
                    return string.Join(SPLITER, this.e, this.s1, this.s2);

                case DirtyElementType.UserBriefInfo:
                    return string.Join(SPLITER, this.e, this.s1);


                #endregion auto_toString

                default:
                    throw new Exception("Unknown this.e " + this.e);
            }
        }

        public static stDirtyElement FromString(string str)
        {
            int index = str.IndexOf(SPLITER);

            stDirtyElement self = new stDirtyElement();
            if (index < 0)
            {
                self.e = Enum.Parse<DirtyElementType>(str);
            }
            else
            {
                self.e = Enum.Parse<DirtyElementType>(str.Substring(0, index));
            }

            switch (self.e)
            {
                #region auto_fromString

                case DirtyElementType.AccountInfo:
                    {
                        int index2 = str.IndexOf(SPLITER, index + 1);
                        self.s1 = str.Substring(index + 1, index2 - index - 1);
                        self.s2 = str.Substring(index2 + 1);
                    }
                    break;

                case DirtyElementType.UserBriefInfo:
                    self.s1 = str.Substring(index + 1);
                    break;


                #endregion auto_fromString

                default:
                    throw new Exception("Unknown self.e " + self.e);
            }

            return self;
        }
    }
}