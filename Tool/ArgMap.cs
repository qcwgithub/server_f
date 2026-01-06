public class ArgMap
{
    public static ArgMap Dictionary<string, string> FromArgs(string[] args)
    {
        var dict = new Dictionary<string, string>();

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
            dict[arg.Substring(0, i)] = arg.Substring(i + 1);
        }
        return dict;
    }

    static string TryGetArg(Dictionary<string, string> argMap, string key)
    {
        string value;
        if (!argMap.TryGetValue(key, out value))
        {
            return null;
        }
        return value;
    }
}