using System;
using System.Collections.Generic;

public static class AskHelp
{
    public static AskSelect AskSelect(string question, params string[] moreOptions)
    {
        var list = new List<string>();
        list.AddRange(moreOptions);
        return new AskSelect(question, list);
    }

    public static AskSelectById AskSelectById(string question, List<AskSelectById.Option> options)
    {
        var list = new List<AskSelectById.Option>();
        list.AddRange(options);
        return new AskSelectById(question, list);
    }

    public static AskInput AskInput(string question)
    {
        return new AskInput(question);
    }
}