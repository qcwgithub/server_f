namespace Data
{
    public struct stDirtyElementWithTime
    {
        public string dirtyElement;
        public int timeS;

        public static stDirtyElementWithTime FromString(string str)
        {
            int i = str.LastIndexOf('@');
            var self = new stDirtyElementWithTime();
            self.dirtyElement = str.Substring(0, i);
            self.timeS = int.Parse(str.Substring(i + 1));
            return self;
        }

        public static string sToString(string dirtyElement, long timeS)
        {
            return dirtyElement + "@" + timeS;
        }
    }
}