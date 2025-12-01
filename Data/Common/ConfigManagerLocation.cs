namespace Data
{
    public class ConfigManagerLocation
    {
        public int serviceId;
        public string inIp;
        public int inPort;

        public void Init()
        {
            if (this.serviceId <= 0)
            {
                Program.LogStartError("this.serviceId <= 0)");
                return;
            }

            if (string.IsNullOrEmpty(this.inIp))
            {
                Program.LogStartError("string.IsNullOrEmpty(this.inIp)");
                return;
            }

            if (this.inPort <= 0)
            {
                Program.LogStartError("this.inPort <= 0");
                return;
            }
        }
    }
}