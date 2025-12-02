using System;
namespace Data
{
    public static class DirtyElementTypeExt
    {
        static void Assert(DirtyElementType e, int correctValue)
        {
            if ((int)e != correctValue)
            {
                throw new Exception(e.ToString() + " != " + correctValue);
            }
        }
        public static void CheckValueNotChange()
        {
            
        }
    }

    public enum DirtyElementType
    {
        // ToString 会存在 redis 中，不能随意改名

        #region auto_enum
        

        #endregion auto_enum
    }
}