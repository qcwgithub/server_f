namespace Data
{
    public class LockOptions
    {
        public int lockTimeS;
        public bool retry;

        List<string> keys;
        bool dirty = false;
        public void AddKey(string key)
        {
            this.dirty = true;
            if (this.keys == null)
            {
                this.keys = new List<string>();
            }
            if (!this.keys.Contains(key))
            {
                this.keys.Add(key);
            }
        }

        public bool IsEmpty()
        {
            return this.keys == null || this.keys.Count == 0;
        }

        string[] keysArray;
        public string[] GetKeys()
        {
            if (this.keysArray == null || this.dirty)
            {
                this.dirty = false;

                this.keysArray = new string[this.keys.Count];
                for (int i = 0; i < this.keys.Count; i++)
                {
                    this.keysArray[i] = this.keys[i];
                }
            }

            return this.keysArray;
        }
        public long startS;
    }

    public class LockOptionsManually : LockOptions
    {
        public bool locked;
    }
}