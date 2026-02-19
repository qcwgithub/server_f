namespace Data
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoRegisterAttribute : Attribute
    {
        public bool isOverride;
        public AutoRegisterAttribute(bool isOverride = false)
        {
            this.isOverride = isOverride;
        }
    }
}