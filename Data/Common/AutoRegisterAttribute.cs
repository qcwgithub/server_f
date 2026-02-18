namespace Data
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoRegisterAttribute : Attribute
    {
        public bool isOverride;
        public AutoRegisterAttribute(bool isOverride)
        {
            this.isOverride = isOverride;
        }
    }
}