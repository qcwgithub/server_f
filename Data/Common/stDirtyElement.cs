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

        #endregion auto_create

        public const char SPLITER = '@';

        public override string ToString()
        {
            switch (this.e)
            {
                #region auto_toString


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


                #endregion auto_fromString

                default:
                    throw new Exception("Unknown self.e " + self.e);
            }

            return self;
        }
    }
}