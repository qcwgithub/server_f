namespace Tool
{
    public partial class LinuxProgram
    {
        void ViewServices()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    Console.WriteLine("Not supported on Windows");
                    break;

                case PlatformID.Unix:
                    {
                        Console.WriteLine("Current running services:");
                        List<string[]> runningServices = this.GetRunningServices();
                        if (true/*groupByProcess*/)
                        {
                            string[] options = runningServices.Select(array => string.Join(',', array)).ToArray();
                            for (int i = 0; i < options.Length; i++)
                            {
                                Console.WriteLine("{0}) {1}", i + 1, options[i]);
                            }
                        }
                    }
                    break;
            }
        }
    }
}