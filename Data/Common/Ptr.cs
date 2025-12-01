namespace Data
{
    public class Ptr<T>
    {
        public T value;
        public void Reset()
        {
            this.value = default(T);
        }
    }
}