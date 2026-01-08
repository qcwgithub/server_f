namespace Tool
{
    public partial class LinuxProgram
    {
        /*

        USER         PID %CPU %MEM    VSZ   RSS TTY      STAT START   TIME COMMAND
        jszuiqcb  314425  2.2  1.2 4256100 102300 ?      Sl   Feb07  23:19 dotnet ./Data.dll scriptDll=./Script.dll server=ca_202311 services=DBAccount70,DBPlayer80
        jszuiqcb  314427  2.1  1.1 4121028 95204 ?       Sl   Feb07  22:58 dotnet ./Data.dll scriptDll=./Script.dll server=ca_202311 services=AAA2
        jszuiqcb  314429  2.1  1.0 4114488 84088 ?       Sl   Feb07  23:01 dotnet ./Data.dll scriptDll=./Script.dll server=ca_202311 services=Arena40
        jszuiqcb  314431  2.1  1.0 4180052 82144 ?       Sl   Feb07  22:56 dotnet ./Data.dll scriptDll=./Script.dll server=ca_202311 services=Arena41
        jszuiqcb  314433  2.1  0.9 4114136 77292 ?       Sl   Feb07  23:01 dotnet ./Data.dll scriptDll=./Script.dll server=ca_202311 services=Union30
        jszuiqcb  314435  2.1  0.9 4106000 78280 ?       Sl   Feb07  22:54 dotnet ./Data.dll scriptDll=./Script.dll server=ca_202311 services=Union31
        jszuiqcb  314437  2.1  1.5 4119360 124360 ?      Sl   Feb07  22:55 dotnet ./Data.dll scriptDll=./Script.dll server=ca_202311 services=Battle50
        jszuiqcb  314442  2.1  1.4 4105320 113932 ?      Sl   Feb07  22:53 dotnet ./Data.dll scriptDll=./Script.dll server=ca_202311 services=Battle51
        jszuiqcb  314444  8.2  1.1 4179648 88308 ?       Sl   Feb07  87:03 dotnet ./Data.dll scriptDll=./Script.dll server=ca_202311 services=Persistence60
        jszuiqcb  314446  8.1  1.0 4179584 86740 ?       Sl   Feb07  86:28 dotnet ./Data.dll scriptDll=./Script.dll server=ca_202311 services=Persistence61
        jszuiqcb  314449  2.1  1.7 4507860 139124 ?      Sl   Feb07  23:10 dotnet ./Data.dll scriptDll=./Script.dll server=ca_202311 services=Player20
        jszuiqcb  314452  2.1  1.7 4041408 139284 ?      Sl   Feb07  23:14 dotnet ./Data.dll scriptDll=./Script.dll server=ca_202311 services=Player21
        jszuiqcb  314457  2.1  1.7 4115120 138864 ?      Sl   Feb07  23:11 dotnet ./Data.dll scriptDll=./Script.dll server=ca_202311 services=Player22
        jszuiqcb  314468  2.4  1.1 4179472 94360 ?       Sl   Feb07  25:28 dotnet ./Data.dll scriptDll=./Script.dll server=ca_202311 services=WorldMap90
        jszuiqcb  314477  2.3  1.1 4245044 94260 ?       Sl   Feb07  25:12 dotnet ./Data.dll scriptDll=./Script.dll server=ca_202311 services=WorldMap91
        jszuiqcb  314484  2.4  1.2 4179216 96640 ?       Sl   Feb07  25:32 dotnet ./Data.dll scriptDll=./Script.dll server=ca_202311 services=WorldMap92
        jszuiqcb  314504  2.1  1.0 4114332 81200 ?       Sl   Feb07  23:01 dotnet ./Data.dll scriptDll=./Script.dll server=ca_202311 services=Global9
        jszuiqcb  314510  2.1  1.0 4116144 81624 ?       Sl   Feb07  22:55 dotnet ./Data.dll scriptDll=./Script.dll server=ca_202311 services=Pay3
        jszuiqcb 1122731  0.0  0.0 113280  1204 pts/0    S+   01:57   0:00 /bin/bash -c ps aux | grep 'COMMAND\|Data.dll'
        jszuiqcb 1122733  0.0  0.0 112812   980 pts/0    S+   01:57   0:00 grep COMMAND\|Data.dll

        */
        void GetRunningServices_fromOutput(List<string[]> runningServices, string output)
        {
            string[] lines = output.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                int i = line.IndexOf("services=");
                if (i < 0)
                {
                    continue;
                }

                i += "services=".Length;
                int j = i;
                for (; j < line.Length; j++)
                {
                    char c = line[j];
                    if (c == ' ' || c == '\r' || c == '\n')
                    {
                        break;
                    }
                }

                string services = line.Substring(i, j - i);
                runningServices.Add(services.Split(','));
            }
        }

        List<string[]> GetRunningServices()
        {
            var runningServices = new List<string[]>();
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    {
                        string output = ExecuteBashCommand(@"ps aux | grep 'COMMAND\|Data.dll'");
                        this.GetRunningServices_fromOutput(runningServices, output);
                        return runningServices;
                    }

                default:
                    {
                        foreach (var item in this.allServiceConfigs)
                        {
                            runningServices.Add([item.tai.ToString()]);
                        }
                        return runningServices;
                    }
            }
        }
    }
}