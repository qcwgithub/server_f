public class ArgMap
{
    Dictionary<string, string> dict = new();

    public static ArgMap ParseArguments(string[] args)
    {
        var map = new ArgMap();

        for (int j = 0; j < args.Length; j++)
        {
            string arg = args[j];
            if (arg.StartsWith("\"") && !arg.EndsWith("\"") && arg.Length > 1)
            {
                while (true)
                {
                    j++;
                    arg += args[j];
                    if (arg.EndsWith("\""))
                    {
                        break;
                    }
                }
            }

            int i = arg.IndexOf('=');
            if (i < 0)
            {
                throw new Exception("BAD ARGUMENT " + arg);
            }
            map.dict[arg.Substring(0, i)] = arg.Substring(i + 1);
        }
        return map;
    }

    public string? GetArg(string key)
    {
        string value;
        if (!this.dict.TryGetValue(key, out value))
        {
            return null;
        }
        return value;
    }
}